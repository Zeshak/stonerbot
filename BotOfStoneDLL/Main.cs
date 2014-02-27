// Type: BotOfStone.Main
// Assembly: BotOfStoneDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CE60BE4-F644-4C0B-806E-CE4EDD47DF44
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\injector\BotOfStoneDLL.dll

namespace BotOfStone
{
  public class Main
  {
    public static void Start()
    {
      Watchdog watchdog = new Watchdog();
      watchdog.startWatch();
      watchdog.RunPlugin("Bot");
    }
  }
}
