using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYClient.Game;
using TeamYShared.Networking;
using UnityEngine;

namespace TeamYClient.Networking
{
    public class NetworkManagement
    {
        private bool _init;
        private ClientEndpoint _client;
        private GameData _data;

        public bool IsConnected => _client?.IsConnected ?? false;

        public NetworkManagement(GameData gameData)
        {
            _data = gameData;
        }

        public void Initialize()
        {
            if(_init)
            {
                return;
            }

            PacketProtocol.RegisterPackets();
            ClientPacketDispatcher dispatcher = new ClientPacketDispatcher();

            _client = new ClientEndpoint(dispatcher, appIdentifier: "TeamY");

            _init = true;
        }

        public void Connect()
        {
            //_client.Connect("127.0.0.1", 14242);
        }

        //When entering the main menu, it is possible we came back from an online editor session. Handle it accordingly.
        public void OnEnteredMainMenu()
        {
            if(_data.State == GameState.OnlineEditor || _client.IsConnected)
            {
                try
                {
                    _client.Disconnect();
                }
                catch
                {
                    //Something went wrong during disconnect. Make sure this client state is completely reset!
                }
            }

            //ClearAllOnlineDataAndAnythingOnlineRelated() (Users , chat , EditorStateData etc)
        }

        public void SendBlockCreate(ulong steamID, string blockJSONAfter)
        {
            Debug.Log($"{steamID} | Create | {blockJSONAfter}");
        }

        public void SendBlockUpdate(ulong steamID, string blockJSONAfter)
        {
            Debug.Log($"{steamID} | Update | {blockJSONAfter}");
        }

        public void SendBlockDestroy(ulong steamID, string blockJSONBefore)
        {
            Debug.Log($"{steamID} | Destroy | {blockJSONBefore}");
        }

        public void SendFloorUpdate(ulong steamID, int floorAfter)
        {
            Debug.Log($"{steamID} | Floor | {floorAfter}");
        }

        public void SendSkyboxUpdate(ulong steamID, string skyboxJSONAfter)
        {
            Debug.Log($"{steamID} | Skybox | {skyboxJSONAfter}");
        }

        public void SendSelection(ulong steamID, string blockUID)
        {
            Debug.Log($"{steamID} | Selection | {blockUID}");
        }

        public void SendDeselection(ulong steamID, string blockUID)
        {
            Debug.Log($"{steamID} | Deselection | {blockUID}");
        }
    }
}
