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
        public bool IsConnected => ServerConnection != null;

        public ClientEndpoint(PacketDispatcher dispatcher, string appIdentifier) : base(dispatcher)
        {
            NetPeerConfiguration config = new NetPeerConfiguration(appIdentifier)
            {
                AcceptIncomingConnections = false
            };

            Peer = new NetPeer(config);
            Peer.Start();
        }

        /// <summary>
        /// Try connection to a server endpoint.
        /// </summary>
        /// <param name="host">The ip address</param>
        /// <param name="port">The port</param>
        public void Connect(string host, int port)
        {
            if(ServerConnection != null)
            {
                return;
            }

            NetOutgoingMessage hail = Peer.CreateMessage("Client connecting");
            ServerConnection = Peer.Connect(host, port, hail);
        }

        /// <summary>
        /// Disconnected from the currently connected server.
        /// </summary>
        /// <param name="reason"></param>
        public void Disconnect(string reason = "Client disconnected")
        {
            Peer?.Shutdown(reason);
            ServerConnection = null;
        }

        /// <summary>
        /// Called when the connection peers status changes (connected / disconnected)
        /// </summary>
        /// <param name="connection">Server connection</param>
        /// <param name="status">The NetConnectionStatus</param>
        /// <param name="reason">Reason for disconnection</param>
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

        /// <summary>
        /// Send a packet to the server.
        /// </summary>
        /// <typeparam name="T">A struct of type IPacket</typeparam>
        /// <param name="packet">The packet to send</param>
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
