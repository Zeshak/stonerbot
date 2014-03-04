using System;
using System.Configuration;
using System.IO;

namespace Hearthstone
{
    public class Log
    {
        public static string logPath = @"D:\StonerBot\logsigmund.txt";
        public static string module = "";

        static Log()
        {
        }

        public static void newLog()
        {
            System.IO.File.Delete(Log.logPath);
        }

        public static void debug(string msg)
        {
            Log.log("DEBUG: " + msg);
        }

        public static void error(Exception ex)
        {
            try
            {
                Log.log("ERROR MESSAGE: " + ex.Message);
                Log.log("ERROR STACKTRACE: " + ex.StackTrace);
                Log.log("ERROR SOURCE: " + ex.Source);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + Log.module + " - ERROR : " + ex.Message);
            }
        }

        public static void error(string msg)
        {
            try
            {
                Log.log("ERROR: " + msg);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + Log.module + " - ERROR : " + msg);
            }
        }

        public static void log(string msg)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + Log.module + " - " + msg);
            System.IO.File.AppendAllText(Log.logPath, DateTime.Now.ToLongTimeString() + ": " + Log.module + " - " + msg + Environment.NewLine);
        }

        public static void log(int msg)
        {
            Log.log(msg.ToString());
        }

        public static void say(string msg)
        {
            try
            {
                Log.log("SAY: " + msg);
                UIStatus.Get().AddInfo(msg);
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + Log.module + " - " + msg);
            }
        }
    }
}
