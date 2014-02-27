// Type: BotOfStone.Log
// Assembly: Bot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E3A6658-12BF-4012-BBAC-A043D3FC1901
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\plugins\Bot.dll

using System;

namespace BotOfStone
{
  public class Log
  {
    public static string logPath = "log.txt";
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
      Log.log(msg);
    }

    public static void error(string msg)
    {
      try
      {
        Log.log("ERROR : " + msg);
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
        Log.log(msg);
        UIStatus.Get().AddInfo(msg);
      }
      catch
      {
        Console.WriteLine(DateTime.Now.ToLongTimeString() + ": " + Log.module + " - " + msg);
      }
    }
  }
}
