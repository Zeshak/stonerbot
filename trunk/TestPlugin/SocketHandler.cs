using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace Plugin
{
    public static class SocketHandler
    {
        private static Thread socket;
        private static TcpClient client;
        private static TcpListener serverSocket;
        private static NetworkStream netStream;
        private static string lastStatus;

        public static void socketThread()
        {
            SocketHandler.serverSocket = new TcpListener(8888);
            SocketHandler.serverSocket.Start();
            while (true)
            {
                try
                {
                    do
                    {
                        do
                            ;
                        while (!SocketHandler.ListenSocket(SocketHandler.serverSocket));
                        SocketHandler.netStream = SocketHandler.client.GetStream();
                    }
                    while (!SocketHandler.netStream.CanRead);
                    byte[] numArray = new byte[SocketHandler.client.ReceiveBufferSize];
                    SocketHandler.netStream.Read(numArray, 0, SocketHandler.client.ReceiveBufferSize);
                    SocketHandler.socketComputing(Encoding.UTF8.GetString(numArray));
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    Log.debug(ex.StackTrace);
                }
            }
        }

        public static void socketComputing(string data)
        {
            if (data.Contains("startbotranked"))
                Plugin.startBotRanked(null, null, null);
            else if (data.Contains("stopbot"))
                Plugin.stopBot(null, null, null);
            else if (data.Contains("finishthisgame"))
                Plugin.finishThisGame(null, null, null);
            else if (data.Contains("startbot"))
                Plugin.StartBot(null, null, null);
            else if (data.Contains("startvsai"))
                Plugin.StartBotVsAI(null, null, null);
            return;
        }

        public static bool ListenSocket(TcpListener serverSocket)
        {
            if (SocketHandler.client != null)
            {
                if (SocketHandler.client.Connected)
                    return true;
                SocketHandler.client.Close();
                ((Stream)SocketHandler.netStream).Close();
                SocketHandler.client = serverSocket.AcceptTcpClient();
                Log.say("Remote connected");
                return true;
            }
            else
            {
                SocketHandler.client = serverSocket.AcceptTcpClient();
                Log.say("Remote connected");
                return true;
            }
        }

        public static void SocketSendStatus(string status)
        {
            if (status.Equals(SocketHandler.lastStatus))
                return;
            SocketHandler.SocketSendCmd(status);
            SocketHandler.lastStatus = status;
        }

        public static void SocketSendCmd(string cmd)
        {
            if (SocketHandler.client == null || SocketHandler.netStream == null || (!SocketHandler.client.Connected || !SocketHandler.netStream.CanWrite))
                return;
            SocketHandler.netStream.WriteTimeout = 1;
            byte[] bytes = Encoding.UTF8.GetBytes(cmd);
            SocketHandler.netStream.Write(bytes, 0, bytes.Length);
            SocketHandler.netStream.Flush();
        }
    }
}
