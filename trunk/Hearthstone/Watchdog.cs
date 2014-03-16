using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hearthstone
{
    public class Watchdog
    {
        static string pluginDirectory = @"D:\StonerBot\plugins";
        Loader loader;
        Hashtable mtimeDb = new Hashtable();

        public void startWatch()
        {
            // init
            loader = new Loader();

            // If a plugin is modified, reload it
            FileSystemWatcher fsw = new FileSystemWatcher(pluginDirectory, "*.dll");
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Changed += new FileSystemEventHandler(onChange_raw);
            fsw.Created += new FileSystemEventHandler(onChange_raw);
            fsw.EnableRaisingEvents = true;
            Log.debug("Watching filesystem for plugin changes in: " + pluginDirectory);
        }

        public bool RunPlugin(string pluginName)
        {
            string path = pluginDirectory + Path.DirectorySeparatorChar + pluginName + ".dll";
            loader.exec(path);
            return true;
        }

        public void onChange_raw(object sender, FileSystemEventArgs e)
        {
            TimeSpan eps = new TimeSpan(0, 0, 2);
            var mtime = File.GetLastWriteTime(e.FullPath);
            if (!mtimeDb.ContainsKey(e.FullPath) || (DateTime)mtimeDb[e.FullPath] + eps < mtime)
            {
                mtimeDb[e.FullPath] = mtime;
                onChange(e.FullPath);
            }
        }

        public void onChange(string path)
        {
            Log.debug("Watchdog: change detected for: " + path);
            Thread.Sleep(1000 * 2); // wait for file to finish writing
            loader.exec(path);
        }
    }
}
