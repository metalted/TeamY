using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Networking;
using TeamYShared.Packets.Development;

namespace TeamYServer.Networking
{
    public class ServerPacketDispatcher : PacketDispatcher
    {
        public ServerPacketDispatcher()
        {
            Register<StringPacket>(OnStringPacket);
        }

        private void OnStringPacket(NetworkEndpoint endpoint, StringPacket packet, NetConnection conn)
        {
            Console.WriteLine($"Received StringPacket: {packet.Data}");

            if(endpoint is ServerEndpoint server)
            {
                server.Send(new StringPacket
                {
                    Data = $"Server received: {packet.Data}"
                }, conn);
            }
        }
    }
}
