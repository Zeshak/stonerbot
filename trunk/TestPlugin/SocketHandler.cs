using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Plugin
{
    public static class SocketHandler
    {
        private class SocketHelper
        {
            TcpClient mscClient;
            string message;
            string response;
            byte[] bytesSent;

            public void processMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
            {
                //Maneja el mensaje recibido y le envía una respuesta.
                message = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
                mscClient = client;
                message = message.Substring(0, 5);
                switch (message)
                {
                    case "stbot":
                        Plugin.StartBot(null, null, null);
                        break;
                    case "stran":
                        Plugin.StartBotRanked(null, null, null);
                        break;
                    case "stain":
                        Plugin.StartBotVsAI(null, null, null);
                        break;
                    case "staie":
                        Plugin.StartBotVsAIExpert(null, null, null);
                        break;
                    case "stopb":
                        Plugin.StopBot(null, null, null);
                        break;
                    case "stopa":
                        Plugin.FinishThisGame(null, null, null);
                        break;
                    default:
                        response = "Error";
                        break;
                }
                if (response != "Error")
                    response = message;
                Log.debug("El mensaje era: " + message);
                bytesSent = Encoding.ASCII.GetBytes(response);
                stream.Write(bytesSent, 0, bytesSent.Length);
            }
        }

        public static void InitSocketListener()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener tcpListener = new TcpListener(ip, 8888);
            tcpListener.Start();
            Log.debug(" >> Server Started");

            while ((true))
            {
                Thread.Sleep(10);
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Log.debug(" >> Accept connection from client");
                byte[] bytes = new byte[256];
                NetworkStream stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                SocketHelper helper = new SocketHelper();
                helper.processMsg(tcpClient, stream, bytes);
            }
        }
    }
}
