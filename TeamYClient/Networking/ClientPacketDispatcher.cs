using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Networking;
using TeamYShared.Packets.Development;
using UnityEngine;

namespace TeamYClient.Networking
{
    public class ClientPacketDispatcher : PacketDispatcher
    {
        public ClientPacketDispatcher()
        {
            Register<StringPacket>(OnStringPacket);
        }

        private void OnStringPacket(NetworkEndpoint endpoint, StringPacket packet, NetConnection conn)
        {
            Debug.Log($"[CLIENT] Received StringPacket: {packet.Data}");
        }
    }
}
