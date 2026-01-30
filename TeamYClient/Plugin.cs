using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYClient.Game;
using TeamYClient.Networking;
using TeamYClient.UI;
using TeamYShared;
using TeamYShared.Networking;
using TeamYShared.Packets.Development;

/*
    NOTES:
    GameObserver.OnCTRLZApplyBefore.case.Skybox: After and before might be switched? Doesnt match the other patterns.
*/

namespace TeamYClient
{
    [BepInPlugin("com.metalted.zeepkist.teamy", "TeamYClient", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public GameData GameData { get; private set; }
        public GameModifier GameModifier { get; private set; }
        public GameObserver GameObserver { get; private set; }
        public LocalPlayerTracker LocalPlayerTracker { get; private set; }
        public NetworkManagement NetworkManagement { get; private set; }
        public UIManagement UIManagement { get; private set; }
        public SelectionObserver SelectionObserver { get; set; }

        public void Awake()
        {
            Instance = this;

            GameData = new GameData();
            GameModifier = new GameModifier(GameData);            
            NetworkManagement = new NetworkManagement(GameData);
            UIManagement = new UIManagement();
            LocalPlayerTracker = new LocalPlayerTracker();

            GameObserver = new GameObserver(GameData, GameModifier, NetworkManagement, UIManagement, LocalPlayerTracker);            

            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            Logger.LogInfo("TeamY plugin loaded");
        }        
    }
}
