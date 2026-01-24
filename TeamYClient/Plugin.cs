using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared;
using HarmonyLib;

namespace TeamYClient
{
    [BepInPlugin("com.metalted.zeepkist.teamy", "TeamYClient", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo("TeamY plugin loaded");
        }
    }
}
