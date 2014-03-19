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
            string response = string.Empty;
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
                        {
                            int length = Convert.ToInt32(message.Substring(5, 2));
                            string messageSay = message.Substring(7, length);
                            Log.say(messageSay);
                            break;
                        }
                    case "state":
                        response = messageb + Plugin.BotStatus.ToString().Length.ToString().PadLeft(2, '0') + Plugin.BotStatus.ToString();
                        break;
                    case "debug":
                        {
                            int length = Convert.ToInt32(message.Substring(5, 2));
                            string messageSay = message.Substring(7, length);
                            Log.debug(@"return " + messageSay + ";");
                            //var evalFunction = EvalProvider.CreateEvalMethod<int, string>(@"return " + messageSay + ";");                            
                            //Log.debug(evalFunction(0));
                            break;
                        }
                    case "anamy":
                        {
                            int pos;
                            if (int.TryParse(message.Substring(5, 1), out pos))
                                Plugin.AnalyzeMyHand(null, null, pos.ToString());
                            else
                                Plugin.AnalyzeMyHand(null, null, null);
                            break;
                        }
                    default:
                        response = "Error";
                        break;
                }
                if (response == "")
                    response = messageb;
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
                byte[] bytes = new byte[256];
                stream = tcpClient.GetStream();
                stream.Read(bytes, 0, bytes.Length);
                SocketHelper helper = new SocketHelper();
                helper.processMsg(tcpClient, stream, bytes);
            }
        }
    }
}
