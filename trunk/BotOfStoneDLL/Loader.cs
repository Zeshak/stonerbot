// Type: BotOfStone.Loader
// Assembly: BotOfStoneDLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2CE60BE4-F644-4C0B-806E-CE4EDD47DF44
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\injector\BotOfStoneDLL.dll

using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;

namespace BotOfStone
{
  public class Loader
  {
    public void exec(string path)
    {
      Log.log("Loader.exec called for " + path);
      string tempFileName = Path.GetTempFileName();
      AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);
      string name = assemblyDefinition.Name.Name;
      assemblyDefinition.Name.Name = name + "_" + Path.GetFileName(tempFileName);
      assemblyDefinition.MainModule.Name = name + "_" + Path.GetFileName(tempFileName);
      assemblyDefinition.Write(tempFileName);
      this.runAssembly(Assembly.LoadFile(tempFileName), name);
    }

    public void runAssembly(Assembly a, string trueName)
    {
      System.Type type = a.GetType("BotOfStone.Plugin");
      object instance = Activator.CreateInstance(type);
      type.InvokeMember("init", BindingFlags.InvokeMethod, (Binder) null, instance, new object[0]);
      Log.debug("Loader.runAssembly ran init for " + trueName);
      Log.debug(type.ToString());
    }
  }
}
