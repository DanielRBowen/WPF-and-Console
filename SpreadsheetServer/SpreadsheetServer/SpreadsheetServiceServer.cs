using CustomNetworking;
using SS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace SpreadsheetServer
{
    internal sealed class SpreadsheetServiceServer : IComparable, IDisposable
    {
        private const string SpreadsheetXmlVersion = "ps6";

        private Socket mySocket;
        private readonly StringSocket myStringSocket;

        private string myPreviousCommandName;
        private string myCommandName;
        private readonly SortedDictionary<string, string> myCommandParameters = new SortedDictionary<string, string>();

        public readonly Guid myID = Guid.NewGuid();
        public Guid ID { get { return myID; } }

        private static bool IsValidCellName(string cellName)
        {
            return true;
        }

        private static string NormalizeCellName(string cellName)
        {
            if (string.IsNullOrWhiteSpace(cellName))
            {
                return null;
            }
            return cellName.ToUpperInvariant();
        }

        private void CreateSession(string name, string password)
        {
            var sessionInfo = new SessionInfo
            {
                SpreadsheetName = name,
                Password = password,
                //Spreadsheet = new Spreadsheet(IsValidCellName, NormalizeCellName, SpreadsheetXmlVersion),
                Spreadsheet = new Spreadsheet(),
                SpreadsheetVersion = 0,
                SpreadsheetUnsavedChanges = new Stack<UnsavedChange>(),
                Clients = new SortedSet<SpreadsheetServiceServer>()
            };

            var sessionInfos = Program.SessionInfos;

            if (sessionInfos.GetOrAdd(name, sessionInfo) != sessionInfo)
            {
                throw new InvalidOperationException("a spreadsheet with the name \"" + name + "\" already exists.");
            }

            Send("CREATE OK\nName:" + name + "\nPassword:" + password + "\n");

            return;
        }

        private void JoinSession(string name, string password)
        {
            var sessionInfos = Program.SessionInfos;
            SessionInfo sessionInfo;

            if (!sessionInfos.TryGetValue(name, out sessionInfo))
            {
                throw new InvalidOperationException("no spreadsheet exists with the name \"" + name + "\".");
            }

            if (password != sessionInfo.Password)
            {
                throw new InvalidOperationException("invalid password.");
            }

            var xml = new StringBuilder();
            uint spreadsheetVersion;

            using (var xmlWriter = XmlWriter.Create(xml))
            {
                lock (sessionInfo.SpreadsheetLock)
                {
                    sessionInfo.Spreadsheet.Save(xmlWriter);
                    spreadsheetVersion = sessionInfo.SpreadsheetVersion;

                    var clients = sessionInfo.Clients;

                    if (!clients.Contains(this))
                    {
                        clients.Add(this);
                    }
                }
            }

            Send("JOIN OK\nName:" + name + "\nVersion:" + spreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo) + "\nLength:" + xml.Length + "\n" + xml.ToString() + "\n");

            return;
        }

        private void NotifyCellChanged
        (
            string spreadsheetName,
            string spreadsheetVersion,
            string cellName,
            string cellContents,
            IEnumerable<SpreadsheetServiceServer> clients
        )
        {
            var message = "UPDATE\nName:" + spreadsheetName +
                "\nVersion:" + spreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo) +
                "\nCell:" + cellName +
                "\nLength:" + cellContents.Length.ToString(NumberFormatInfo.InvariantInfo) +
                "\n" + cellContents +
                "\n";

            foreach (var client in clients)
            {
                if (client != this)
                {
                    try
                    {
                        client.Send(message);
                    }
                    catch
                    {
                    }
                }
            }

            return;
        }

        private void ChangeCell(string name, string version, string cell, string length, string content)
        {
            var sessionInfos = Program.SessionInfos;
            SessionInfo sessionInfo;

            if (!sessionInfos.TryGetValue(name, out sessionInfo))
            {
                throw new InvalidOperationException("no spreadsheet exists with the name \"" + name + "\".");
            }

            int lengthValue;

            if (!int.TryParse(length, out lengthValue) || lengthValue < 1)
            {
                throw new InvalidOperationException("\"" + length + "\" isn't a valid length.");
            }

            var contentLength = content.Length;

            if (lengthValue != contentLength)
            {
                throw new InvalidOperationException("length \"" + length + "\" was expected to be \"" + contentLength.ToString(NumberFormatInfo.InvariantInfo) + "\".");
            }

            uint currentSpreadsheetVersion;
            ISet<SpreadsheetServiceServer> clients;

            lock (sessionInfo.SpreadsheetLock)
            {
                currentSpreadsheetVersion = sessionInfo.SpreadsheetVersion;
                var currentSpreadsheetVersionString = currentSpreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo);

                if (version != currentSpreadsheetVersionString)
                {
                    Send("CHANGE WAIT\nName:" + name + "\nVersion:" + currentSpreadsheetVersionString + "\n");
                    return;
                }

                var spreadsheet = sessionInfo.Spreadsheet;
                var previousCellContents = string.Empty;

                try
                {
                    previousCellContents = spreadsheet.GetCellContents(cell).ToString();
                }
                catch
                {
                }

                spreadsheet.SetContentsOfCell(cell, content);

                var unsavedChange = new UnsavedChange
                {
                    CellName = cell,
                    PreviousCellContents = previousCellContents
                };

                sessionInfo.SpreadsheetUnsavedChanges.Push(unsavedChange);
                sessionInfo.SpreadsheetVersion = ++currentSpreadsheetVersion;
                clients = sessionInfo.Clients;
            }

            version = currentSpreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo);
            Send("CHANGE OK\nName:" + name + "\nVersion:" + version + "\n");
            NotifyCellChanged(name, version, cell, content, clients);

            return;
        }

        private void UndoLastChange(string name, string version)
        {
            var sessionInfos = Program.SessionInfos;
            SessionInfo sessionInfo;

            if (!sessionInfos.TryGetValue(name, out sessionInfo))
            {
                throw new InvalidOperationException("no spreadsheet exists with the name \"" + name + "\".");
            }

            uint currentSpreadsheetVersion;
            string cellName;
            string previousCellContents;
            ISet<SpreadsheetServiceServer> clients;

            lock (sessionInfo.SpreadsheetLock)
            {
                currentSpreadsheetVersion = sessionInfo.SpreadsheetVersion;
                var currentSpreadsheetVersionString = currentSpreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo);

                if (version != currentSpreadsheetVersionString)
                {
                    Send("UNDO WAIT\nName:" + name + "\nVersion:" + currentSpreadsheetVersionString + "\n");
                    return;
                }

                var unsavedChanges = sessionInfo.SpreadsheetUnsavedChanges;

                if (unsavedChanges.Count == 0)
                {
                    Send("UNDO END\nName:" + name + "\nVersion:" + currentSpreadsheetVersionString + "\n");
                    return;
                }

                var unsavedChange = unsavedChanges.Pop();
                cellName = unsavedChange.CellName;
                previousCellContents = unsavedChange.PreviousCellContents;

                sessionInfo.Spreadsheet.SetContentsOfCell(cellName, previousCellContents);
                sessionInfo.SpreadsheetVersion = ++currentSpreadsheetVersion;
                clients = sessionInfo.Clients;
            }

            version = currentSpreadsheetVersion.ToString(NumberFormatInfo.InvariantInfo);

            var message = "UNDO SP OK\nName:" + name +
                "\nVersion:" + version +
                "\nCell:" + cellName +
                "\nLength:" + previousCellContents.Length.ToString(NumberFormatInfo.InvariantInfo) +
                "\n" + previousCellContents +
                "\n";

            Send(message);
            NotifyCellChanged(name, version, cellName, previousCellContents, clients);

            return;
        }

        private void Save(string name)
        {
            var sessionInfos = Program.SessionInfos;
            SessionInfo sessionInfo;

            if (!sessionInfos.TryGetValue(name, out sessionInfo))
            {
                throw new InvalidOperationException("no spreadsheet exists with the name \"" + name + "\".");
            }

            lock (sessionInfo.SpreadsheetLock)
            {
                sessionInfo.SpreadsheetUnsavedChanges.Clear();
            }

            Send("SAVE OK\nName:" + name + "\n");

            return;
        }

        private void Leave(string name)
        {
            var sessionInfos = Program.SessionInfos;
            SessionInfo sessionInfo;

            if (!sessionInfos.TryGetValue(name, out sessionInfo))
            {
                return;
            }

            lock (sessionInfo.SpreadsheetLock)
            {
                var clients = sessionInfo.Clients;

                if (clients.Contains(this))
                {
                    clients.Remove(this);
                }
            }

            return;
        }

        private void Disconnect()
        {
            var socket = mySocket;

            if (socket != null)
            {
                socket.Dispose();
                mySocket = null;
            }

            Program.TryRemoveServer(this);

            return;
        }

        private void HandleAsyncException(Exception ex)
        {
            Disconnect();

            if (ex is ObjectDisposedException)
            {
                return;
            }

            /*var asyncException = AsyncException;

			if (asyncException != null)
			{
				asyncException(this, new AsyncExceptionEventArgs(ex));
			}*/

            return;
        }

        private void LineSent(Exception ex, object parameters)
        {
            if (ex != null)
            {
                PlayerDisconnected(ex);
                return;
            }

            /*var line = (string)parameters;

			if (line == null)
			{
				return;
			}

			if (line.StartsWith("START ", StringComparison.Ordinal))
			{
				var startSignal = myStartSignal;

				if (startSignal != null)
				{
					startSignal.Set();
				}
			}
			else if (line.StartsWith("STOP ", StringComparison.Ordinal))
			{
				var stopSignal = myStopSignal;

				if (stopSignal != null)
				{
					stopSignal.Set();
				}
			}*/

            return;
        }

        private void Send(string line)
        {
            try
            {
                myStringSocket.BeginSend(line + "\r\n", LineSent, line);
            }
            catch (Exception ex)
            {
                PlayerDisconnected(ex);
            }
            return;
        }

        private bool TryProcessCommand(string previousCommandName, string commandName, SortedDictionary<string, string> commandParameters)
        {
            string name;
            string password;
            string version;
            string cell;
            string length;

            switch (commandName)
            {
                case "create":
                    if (commandParameters.TryGetValue("name", out name) && commandParameters.TryGetValue("password", out password))
                    {
                        try
                        {
                            CreateSession(name, password);
                        }
                        catch (Exception ex)
                        {
                            Send("CREATE FAIL\nName:" + name + "\n" + ex.Message + "\n");
                        }
                        return true;
                    }
                    return false;
                case "join":
                    if (commandParameters.TryGetValue("name", out name) && commandParameters.TryGetValue("password", out password))
                    {
                        try
                        {
                            JoinSession(name, password);
                        }
                        catch (Exception ex)
                        {
                            Send("JOIN FAIL\nName:" + name + "\n" + ex.Message + "\n");
                        }
                        return true;
                    }
                    return false;
                case "undo":
                    if (commandParameters.TryGetValue("name", out name) && commandParameters.TryGetValue("version", out version))
                    {
                        try
                        {
                            UndoLastChange(name, version);
                        }
                        catch (Exception ex)
                        {
                            Send("UNDO FAIL\nName:" + name + "\n" + ex.Message + "\n");
                        }
                        return true;
                    }
                    return false;
                case "save":
                    if (commandParameters.TryGetValue("name", out name))
                    {
                        try
                        {
                            Save(name);
                        }
                        catch (Exception ex)
                        {
                            Send("SAVE FAIL\nName:" + name + "\n" + ex.Message + "\n");
                        }
                        return true;
                    }
                    return false;
                case "leave":
                    if (commandParameters.TryGetValue("name", out name))
                    {
                        try
                        {
                            Leave(name);
                        }
                        catch
                        {
                        }
                        return true;
                    }
                    return false;
                default:
                    switch (previousCommandName)
                    {
                        case "change":
                            if
                            (
                                commandParameters.TryGetValue("name", out name) &&
                                commandParameters.TryGetValue("version", out version) &&
                                commandParameters.TryGetValue("cell", out cell) &&
                                commandParameters.TryGetValue("length", out length)
                            )
                            {
                                try
                                {
                                    ChangeCell(name, version, cell, length, commandName);
                                }
                                catch (Exception ex)
                                {
                                    Send("CHANGE FAIL\nName:" + name + "\n" + ex.Message + "\n");
                                }
                                return true;
                            }
                            return false;
                        default:
                            return false;
                    }
            }
        }

        private void LineReceived(string line, Exception ex, object parameters)
        {
            if (ex != null)
            {
                HandleAsyncException(ex);
                return;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            var commandParameters = myCommandParameters;
            var index = line.IndexOf(':');

            if (index != -1)
            {
                var name = line.Substring(0, index).Trim().ToLowerInvariant();
                var value = line.Substring(index + 1);
                commandParameters[name] = value;
            }
            else
            {
                myPreviousCommandName = myCommandName;
                myCommandName = line.Trim().ToLowerInvariant();
            }

            if (TryProcessCommand(myPreviousCommandName, myCommandName, commandParameters))
            {
                myPreviousCommandName = null;
                myCommandName = null;
                commandParameters.Clear();
            }

            Receive();

            return;
        }

        private void PlayerDisconnected(Exception ex)
        {
            Dispose();
            return;
        }

        private void Receive()
        {
            try
            {
                myStringSocket.BeginReceive(LineReceived, null);
            }
            catch (Exception ex)
            {
                PlayerDisconnected(ex);
            }
            return;
        }

        public SpreadsheetServiceServer(Socket socket)
        {
            mySocket = socket;
            myStringSocket = new StringSocket(socket, Encoding.UTF8);
            Receive();
        }

        public int CompareTo(object obj)
        {
            return myID.CompareTo(((SpreadsheetServiceServer)obj).myID);
        }

        public void Dispose()
        {
            Disconnect();
            return;
        }
    }
}