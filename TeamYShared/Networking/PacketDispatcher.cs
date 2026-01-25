using LidgrenX.Network;
using System;
using System.Collections.Generic;
using TeamYShared.Packets;

namespace TeamYShared.Networking
{
    public class PacketDispatcher
    {
        private readonly Dictionary<Type, Action<NetworkEndpoint, IPacket, NetConnection>> _handlers = new Dictionary<Type, Action<NetworkEndpoint, IPacket, NetConnection>>();

        protected void Register<T>(Action<NetworkEndpoint, T, NetConnection> handler) where T : IPacket
        {
            _handlers[typeof(T)] = (endpoint, packet, conn) =>
                handler(endpoint, (T)packet, conn);
        }

        public void Dispatch(NetworkEndpoint endpoint, IPacket packet, NetConnection conn)
        {
            if (_handlers.TryGetValue(packet.GetType(), out var handler))
            {
                Console.WriteLine($"[DISPATCH] {packet.GetType().Name}");
                handler(endpoint, packet, conn);
            }
            else
            {
                Console.WriteLine($"[DISPATCH] No handler for {packet.GetType().Name}");
            }
        }
    }
}