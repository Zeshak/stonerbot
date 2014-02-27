using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;

namespace StonerBot
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
            runAssembly(Assembly.LoadFile(tempPath), name);
        }

        public void runAssembly(Assembly a, string trueName)
        {
            System.Type type = a.GetType("StonerBot.Plugin");
            object instance = Activator.CreateInstance(type);
            type.InvokeMember("init", BindingFlags.InvokeMethod, (Binder)null, instance, new object[0]);
            Log.debug("Loader.runAssembly ran init for " + trueName);
            Log.debug(type.ToString());
        }
    }
}
