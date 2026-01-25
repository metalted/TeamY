using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYClient.Networking;
using TeamYShared;
using TeamYShared.Networking;
using TeamYShared.Packets.Development;

namespace TeamYClient
{
    [BepInPlugin("com.metalted.zeepkist.teamy", "TeamYClient", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ClientEndpoint _client;

        public void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            PacketProtocol.RegisterPackets();
            ClientPacketDispatcher dispatcher = new ClientPacketDispatcher();

            _client = new ClientEndpoint(dispatcher, appIdentifier: "TeamY");
            _client.Connect("127.0.0.1", 14242);

            Invoke(nameof(SendTestPacket), 2f);

            Logger.LogInfo("TeamY plugin loaded");
        }

        private void Update()
        {
            _client?.Poll();
        }

        private void SendTestPacket()
        {
            Logger.LogInfo("[CLIENT] Sending StringPacket");

            _client.Send(new StringPacket
            {
                Data = "Hello from Client"
            });
        }
    }
}
