// Type: BotOfStone.Plugin
// Assembly: Statistics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1FAA4770-3A04-473C-A1D0-1E1334DF5D03
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\plugins\Statistics.dll

using UnityEngine;

namespace BotOfStone
{
    public class Plugin : MonoBehaviour
    {
        public static void init()
        {
            GameObject gameObject = SceneMgr.Get().gameObject;
            Object.Destroy((Object)gameObject.GetComponent("Plugin"));
            foreach (Object @object in gameObject.GetComponents<Plugin>())
                Object.Destroy(@object);
            gameObject.AddComponent<Plugin>();
        }
    }
}
