using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetClient
{
    internal abstract class TextTcpClient : IDisposable
    {
        private TcpClient myTcpClient;
        private StringSocket myStringSocket;

        private string myPreviousCommandName;
        private string myCommandName;
        private readonly SortedDictionary<string, string> myCommandParameters = new SortedDictionary<string, string>();

        public event AsyncExceptionEventHandler AsyncException;

        private void Disconnect()
        {
            myStringSocket = null;

            var tcpClient = myTcpClient;

            if (tcpClient != null)
            {
                var socket = tcpClient.Client;

                if (socket != null)
                {
                    socket.Dispose();
                }

                tcpClient.Close();
                myTcpClient = null;
            }

            return;
        }

        private void HandleAsyncException(Exception ex)
        {
            Disconnect();

            if (ex is ObjectDisposedException)
            {
                return;
            }

            var asyncException = AsyncException;

            if (asyncException != null)
            {
                asyncException(this, new AsyncExceptionEventArgs(ex));
            }

            return;
        }

        protected abstract bool TryProcessCommand(string previousCommandName, string commandName, SortedDictionary<string, string> commandParameters);

        private void LineReceived(string line, Exception ex, object parameters)
        {
            if (ex != null)
            {
                HandleAsyncException(ex);
                return;
            }

            try
            {
                if (line == null)
                {
                    line = string.Empty;
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
            }
            finally
            {
                Receive();
            }

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
                HandleAsyncException(ex);
            }
            return;
        }

        protected async Task<bool> SendText(string text)
        {
            try
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                myStringSocket.BeginSend(text, (ex, a) => { taskCompletionSource.TrySetResult(true); }, null);
                await taskCompletionSource.Task;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task Connect(string host, int port)
        {
            try
            {
                var tcpClient = new TcpClient();
                myTcpClient = tcpClient;

                await tcpClient.ConnectAsync(host, port);

                var stringSocket = new StringSocket(myTcpClient.Client, Encoding.UTF8);
                myStringSocket = stringSocket;
            }
            catch
            {
                Disconnect();
                throw;
            }

            Receive();

            return;
        }

        public void Dispose()
        {
            Disconnect();
            return;
        }
    }
}
