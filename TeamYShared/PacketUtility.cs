using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared
{
    public class PacketUtility
    {
        private static readonly Dictionary<ushort, Type> PacketTypeRegistry = new Dictionary<ushort, Type>();

        /// <summary>
        /// Automatically registers all packets in the same namespace as <see cref="PacketUtility"/> 
        /// that implement <see cref="IPacket"/> and are structs.
        /// </summary>
        public static void AutoRegisterPacketsInSameNamespace()
        {
            var packetInterface = typeof(IPacket);

            var packetTypes = typeof(PacketUtility).Assembly
                                       .GetTypes()
                                       .Where(t => packetInterface.IsAssignableFrom(t) && t.IsValueType);

            foreach (var type in packetTypes)
            {
                RegisterPacketType(type);
            }
        }

        /// <summary>
        /// Registers a packet type and assigns it a unique ID based on its stable hash code.
        /// </summary>
        /// <param name="packetType">The type of the packet to register.</param>
        public static void RegisterPacketType(Type packetType)
        {
            ushort packetId = (ushort)(packetType.Name.GetStableHashCode() & ushort.MaxValue);
            PacketTypeRegistry[packetId] = packetType;
        }

        /// <summary>
        /// Retrieves the type of a packet using its ID.
        /// </summary>
        /// <param name="packetId">The ID of the packet.</param>
        /// <returns>The type of the packet, or null if not found.</returns>
        public static Type GetPacketType(ushort packetId)
        {
            return PacketTypeRegistry.TryGetValue(packetId, out var type) ? type : null;
        }

        /// <summary>
        /// Gets the packet ID for a given generic packet type.
        /// </summary>
        /// <typeparam name="T">The type of the packet.</typeparam>
        /// <returns>The ID of the packet.</returns>
        public static ushort GetPacketId<T>() where T : struct, IPacket
        {
            string typeName = typeof(T).Name;
            return (ushort)(typeName.GetStableHashCode() & ushort.MaxValue);
        }

        /// <summary>
        /// Generates a stable hash-based packet ID for a given type name.
        /// </summary>
        /// <param name="typeName">The name of the packet type.</param>
        /// <returns>The packet ID.</returns>
        private static ushort GetPacketId(string typeName)
        {
            return (ushort)(typeName.GetStableHashCode() & ushort.MaxValue);
        }

        /// <summary>
        /// Packs a packet into a <see cref="NetOutgoingMessage"/>.
        /// </summary>
        /// <typeparam name="T">The type of the packet.</typeparam>
        /// <param name="packet">The packet to pack.</param>
        /// <param name="outgoingMessage">The outgoing message to populate.</param>
        public static void Pack<T>(T packet, NetOutgoingMessage outgoingMessage) where T : struct, IPacket
        {
            ushort packetId = GetPacketId<T>();
            outgoingMessage.Write(packetId);
            packet.Serialize(outgoingMessage);
        }

        /// <summary>
        /// Unpacks a <see cref="NetIncomingMessage"/> to retrieve the message type.
        /// </summary>
        /// <param name="incomingMessage">The incoming message to unpack.</param>
        /// <param name="msgType">The message type.</param>
        /// <returns>True if unpacking was successful, otherwise false.</returns>
        public static bool Unpack(NetIncomingMessage incomingMessage, out ushort msgType)
        {
            try
            {
                msgType = incomingMessage.ReadUInt16();
                return true;
            }
            catch (Exception ex)
            {
                msgType = 0;
                return false;
            }
        }
    }
}
