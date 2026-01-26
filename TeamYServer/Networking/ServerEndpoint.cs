using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Networking;
using TeamYShared.Packets;

namespace TeamYServer.Networking
{
    public class ServerEndpoint : NetworkEndpoint
    {
        private List<NetConnection> _clients = new List<NetConnection>();

        public IReadOnlyList<NetConnection> Clients => _clients;

        public ServerEndpoint(PacketDispatcher dispatcher, string appIdentifier, int port) : base(dispatcher)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier)
            {
                Port = port,
                AcceptIncomingConnections = true
            };

            Peer = new NetPeer(config);
            Peer.Start();

            Console.WriteLine($"[SERVER] Listening on port {port}");
        }

        protected override void OnStatusChanged(NetConnection connection, NetConnectionStatus status, string reason)
        {
            if(status  == NetConnectionStatus.Connected)
            {
                _clients.Add(connection);
            }
            else if(status == NetConnectionStatus.Disconnected)
            {
                _clients.Remove(connection);
            }
        }

        /// <summary>
        /// Send a packet to a single client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet">The IPacket to send</param>
        /// <param name="client">The client to send it to</param>
        public void Send<T>(T packet, NetConnection client) where T : struct, IPacket
        {
            if(client == null)
            {
                return;
            }

            NetOutgoingMessage om = Peer.CreateMessage();
            PacketUtility.Pack(packet, om);

            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered, 0);
        }

        /// <summary>
        /// Send a packet to all connected clients.
        /// </summary>
        /// <typeparam name="T">Struct of type IPacket</typeparam>
        /// <param name="packet">The IPacket to send</param>
        public void Broadcast<T>(T packet) where T : struct, IPacket
        {
            if(_clients.Count == 0)
            {
                return;
            }

            NetOutgoingMessage om = Peer.CreateMessage();
            PacketUtility.Pack(packet, om);

            Peer.SendMessage(om, _clients, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
