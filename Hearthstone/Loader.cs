using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hearthstone
{
    public class Loader
    {
        public void exec(string path) // creates shadow copy
        {
            Log.log("Loader.exec called for " + path);
            string tempPath = System.IO.Path.GetTempFileName();

            // rewrite name to let us load a different version of the same library
            var ad = Mono.Cecil.AssemblyDefinition.ReadAssembly(path);
            var name = ad.Name.Name;
            ad.Name.Name = name + "_" + Path.GetFileName(tempPath);
            ad.MainModule.Name = name + "_" + Path.GetFileName(tempPath);
            ad.Write(tempPath);

            // load assembly and run the init
            var a = Assembly.LoadFile(tempPath);
            runAssembly(a, name);
        }

        public void runAssembly(Assembly a, string trueName)
        {
            var t = a.GetType("Plugin.Plugin");
            var c = Activator.CreateInstance(t);
            t.InvokeMember("init", BindingFlags.InvokeMethod, null, c, new object[] { });
            Log.debug("Loader.runAssembly ran Plugin.Plugin.init for " + trueName);
        }
    }
}
