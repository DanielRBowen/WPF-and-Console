using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace SpreadsheetServer
{
    public static class Program
    {
        private static readonly ConcurrentDictionary<string, SessionInfo> theSessionInfos = new ConcurrentDictionary<string, SessionInfo>();
        internal static ConcurrentDictionary<string, SessionInfo> SessionInfos { get { return theSessionInfos; } }

        private static ConcurrentDictionary<Guid, SpreadsheetServiceServer> theSpreadsheetServiceServers = new ConcurrentDictionary<Guid, SpreadsheetServiceServer>();
        private static volatile bool Stop;

        internal static bool TryRemoveServer(SpreadsheetServiceServer spreadsheetServiceServer)
        {
            SpreadsheetServiceServer value;
            return theSpreadsheetServiceServers.TryRemove(spreadsheetServiceServer.ID, out value);
        }

        public static void Main(string[] args)
        {
            TcpListener tcpListener = null;
            var spreadsheetServiceServers = theSpreadsheetServiceServers;

            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 1984);
                tcpListener.Start();
                Console.WriteLine("Now Listening");

                while (!Stop)
                {
                    var spreadsheetServiceServer = new SpreadsheetServiceServer(tcpListener.AcceptSocket());

                    if (spreadsheetServiceServer != null)
                    {
                        spreadsheetServiceServers[spreadsheetServiceServer.ID] = spreadsheetServiceServer;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }
            }

            return;
        }
    }
}
