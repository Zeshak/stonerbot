using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;
using System.Configuration;

namespace Injector
{
    class Injector
    {
        static string appOrigPath = ConfigurationManager.AppSettings["appOrigPath"];
        static string injPath = ConfigurationManager.AppSettings["injPath"];
        static string appPatchedPath = ConfigurationManager.AppSettings["appPatchedPath"];

        static string appTypeName = "SceneMgr";
        static string appMethodName = "Start";
        static string injTypeName = "Main";
        static string injMethodName = "Start";
        
        static void Main(string[] args)
        {
            AssemblyDefinition app = AssemblyDefinition.ReadAssembly(appOrigPath);
            AssemblyDefinition inj = AssemblyDefinition.ReadAssembly(injPath);

            TypeDefinition injType = inj.MainModule.Types.Single(t => t.Name == injTypeName);
            MethodDefinition injMethod = injType.Methods.Single(t => t.Name == injMethodName);

            TypeDefinition appType = app.MainModule.Types.Single(t => t.Name == appTypeName);
            MethodDefinition appMethod = appType.Methods.Single(t => t.Name == appMethodName);

            ILProcessor ipl = appMethod.Body.GetILProcessor();
            Instruction firstInstruction = ipl.Body.Instructions[0];
            Instruction instruction = ipl.Create(OpCodes.Call, app.MainModule.Import(injMethod.Resolve()));
            ipl.InsertBefore(firstInstruction, instruction);
            app.Write(appPatchedPath);
        }
    }
}
