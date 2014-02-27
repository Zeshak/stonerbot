// Type: BotOfStone.Watchdog
// Assembly: BotOfStoneDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CE60BE4-F644-4C0B-806E-CE4EDD47DF44
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\injector\BotOfStoneDLL.dll

using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace BotOfStone
{
  internal class Watchdog
  {
    private Hashtable mtimeDb = new Hashtable();
    private static string pluginDirectory;
    private Loader loader;

    public Watchdog()
    {
      Watchdog.pluginDirectory = "D:\\HearthstoneDev\\HearthBot\\plugins";
      try
      {
        Watchdog.pluginDirectory = System.IO.File.ReadAllText("plugins.txt");
      }
      catch (Exception ex)
      {
        Log.debug("Error while reading plugins.txt : " + ex.StackTrace);
      }
    }

    public void startWatch()
    {
      this.loader = new Loader();
      FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Watchdog.pluginDirectory, "*.dll");
      fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
      fileSystemWatcher.Changed += new FileSystemEventHandler(this.onChange_raw);
      fileSystemWatcher.Created += new FileSystemEventHandler(this.onChange_raw);
      fileSystemWatcher.EnableRaisingEvents = true;
      Log.debug("Watching filesystem for plugin changes in: " + Watchdog.pluginDirectory);
      CheatMgr.Get().RegisterCheatHandler("run", new CheatMgr.ProcessCheatCallback(this.RunCommand));
    }

    public bool RunPlugin(string pluginName)
    {
      this.loader.exec(string.Concat(new object[4]
      {
        (object) Watchdog.pluginDirectory,
        (object) System.IO.Path.DirectorySeparatorChar,
        (object) pluginName,
        (object) ".dll"
      }));
      return true;
    }

    public bool RunCommand(string func, string[] args, string rawArgs)
    {
      return this.RunPlugin(rawArgs);
    }

    public void onChange_raw(object sender, FileSystemEventArgs e)
    {
      TimeSpan timeSpan = new TimeSpan(0, 0, 2);
      DateTime lastWriteTime = System.IO.File.GetLastWriteTime(e.FullPath);
      if (this.mtimeDb.ContainsKey((object) e.FullPath) && !((DateTime) this.mtimeDb[(object) e.FullPath] + timeSpan < lastWriteTime))
        return;
      this.mtimeDb[(object) e.FullPath] = (object) lastWriteTime;
      this.onChange(e.FullPath);
    }

    public void onChange(string path)
    {
      Log.say("Watchdog: change detected for: " + path);
      Thread.Sleep(2000);
      this.loader.exec(path);
    }
  }
}
