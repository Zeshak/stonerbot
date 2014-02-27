// Type: BotOfStone.Log
// Assembly: BotOfStoneDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CE60BE4-F644-4C0B-806E-CE4EDD47DF44
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\injector\BotOfStoneDLL.dll

using System;

namespace BotOfStone
{
  public class Log
  {
    public static void debug(string msg)
    {
      Log.log(msg);
    }

    public static void log(string msg)
    {
      Console.WriteLine(msg);
    }

    public static void say(string msg)
    {
      UIStatus.Get().AddInfo(msg);
      Log.log(msg);
    }
  }
}
