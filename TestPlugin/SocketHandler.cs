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
    public class SocketHandler
    {
        public static TcpListener tcpListener;
        public static NetworkStream stream;
        public static TcpClient tcpClient;

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
                string messageb = message.Substring(0, 5);
                switch (messageb)
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
                    case "saywo":
                        int length = Convert.ToInt32(message.Substring(5, 1));
                        string messageSay = message.Substring(6, length);
                        Log.say(messageSay);
                        break;
                    default:
                        response = "Error";
                        break;
                }
                if (response != "Error")
                    response = messageb;
                Log.debug("El mensaje era:  " + messageb);
                bytesSent = Encoding.ASCII.GetBytes(response);
                stream.Write(bytesSent, 0, bytesSent.Length);
            }
        }

        public static void InitSocketListener()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            Thread.Sleep(1000);
            tcpListener = new TcpListener(ip, 8888);
            try
            {
                tcpListener.Start();
                Log.debug(" >> Server Started");
            }
            catch (SocketException)
            {
                Log.debug(" >> Server was Started.");
            }

            while ((true))
            {
                Thread.Sleep(10);
                tcpClient = tcpListener.AcceptTcpClient();
                Log.debug(" >> Accept connection from client");
                byte[] bytes = new byte[256];
                stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                SocketHelper helper = new SocketHelper();
                helper.processMsg(tcpClient, stream, bytes);
            }
        }
    }
}
