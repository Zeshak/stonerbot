using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Hearthstone
{
    public class Main
    {
        public static void Start()
        {
            var w = new Watchdog();
            w.startWatch();
            Log.log("Sigmund online");
            w.RunPlugin("TestPlugin");
        }
    }
}
