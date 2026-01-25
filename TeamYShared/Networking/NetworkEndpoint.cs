using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Packets;

namespace TeamYShared.Networking
{
    public abstract class NetworkEndpoint
    {
        protected PacketDispatcher Dispatcher;
        protected NetPeer Peer;

        protected NetworkEndpoint(PacketDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void Poll()
        {
            if (Peer == null)
                return;

            NetIncomingMessage im;
            while ((im = Peer.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        HandleData(im);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        HandleStatus(im);
                        break;

                    default:
                        // ignore or log
                        break;
                }

                Peer.Recycle(im);
            }
        }

        protected void HandleData(NetIncomingMessage im)
        {
            if (PacketUtility.TryUnpack(im, out IPacket packet))
            {
                Dispatcher.Dispatch(this, packet, im.SenderConnection);
            }
        }

        protected void HandleStatus(NetIncomingMessage im)
        {
            var status = (NetConnectionStatus)im.ReadByte();
            var reason = im.ReadString();

            OnStatusChanged(im.SenderConnection, status, reason);
        }

        protected virtual void OnStatusChanged(NetConnection connection, NetConnectionStatus status, string reason) { }
    }
}
