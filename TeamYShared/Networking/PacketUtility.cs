using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Packets;

namespace TeamYShared.Networking
{
    /// <summary>
    /// Holds the registered packets, and is used to pack and unpack network messages into their respective IPackets.
    /// </summary>
    public static class PacketUtility
    {
        private static readonly Dictionary<ushort, Type> IdToType = new Dictionary<ushort, Type>();
        private static readonly Dictionary<Type, ushort> TypeToId = new Dictionary<Type, ushort>();

        public static void RegisterPacket<T>(ushort id) where T : struct, IPacket
        {
            var type = typeof(T);

            if (IdToType.ContainsKey(id))
            {
                throw new InvalidOperationException($"Packet ID {id} already registered.");
            }

            if (TypeToId.ContainsKey(type))
            {
                throw new InvalidOperationException($"Packet type {type.Name} already registered.");
            }

            IdToType[id] = type;
            TypeToId[type] = id;
        }

        public static void Pack<T>(T packet, NetOutgoingMessage om) where T : struct, IPacket
        {
            if (!TypeToId.TryGetValue(typeof(T), out ushort id))
            {
                throw new InvalidOperationException($"Packet type {typeof(T).Name} is not registered.");
            }

            om.Write(id);
            packet.Serialize(om);
        }

        public static bool TryUnpack(NetIncomingMessage im, out IPacket packet)
        {
            packet = null;

            try
            {
                ushort id = im.ReadUInt16();

                if (!IdToType.TryGetValue(id, out Type type))
                {
                    return false;
                }

                var instance = (IPacket)Activator.CreateInstance(type);
                instance.Deserialize(im);

                packet = instance;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
