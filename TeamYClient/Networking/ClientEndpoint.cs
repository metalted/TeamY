using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Networking;
using TeamYShared.Packets;

namespace TeamYClient.Networking
{
    public class ClientEndpoint : NetworkEndpoint
    {
        public NetConnection ServerConnection { get; private set; }

        public ClientEndpoint(PacketDispatcher dispatcher, string appIdentifier) : base(dispatcher)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier)
            {
                AcceptIncomingConnections = false
            };

            Peer = new NetPeer(config);
            Peer.Start();
        }

        public void Connect(string host, int port)
        {
            if(ServerConnection != null)
            {
                return;
            }

            NetOutgoingMessage hail = Peer.CreateMessage("Client connecting");
            ServerConnection = Peer.Connect(host, port, hail);
        }

        public void Disconnect(string reason = "Client disconnected")
        {
            Peer?.Shutdown(reason);
            ServerConnection = null;
        }

        protected override void OnStatusChanged(NetConnection connection, NetConnectionStatus status, string reason)
        {
            if(status == NetConnectionStatus.Connected)
            {
                ServerConnection = connection;
            }
            else if(status == NetConnectionStatus.Disconnected)
            {
                ServerConnection = null;
            }
        }

        public void Send<T>(T packet) where T : struct, IPacket
        {
            if(ServerConnection == null)
            {
                return;
            }

            NetOutgoingMessage om = Peer.CreateMessage();
            PacketUtility.Pack(packet, om);

            ServerConnection.SendMessage(om, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
