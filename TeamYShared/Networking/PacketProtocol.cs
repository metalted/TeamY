using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamYShared.Packets.Development;

namespace TeamYShared.Networking
{
    /// <summary>
    /// PacketProtocol is the class where Packets are registered and given a unique id.
    /// </summary>
    public static class PacketProtocol
    {
        public static void RegisterPackets()
        {
            PacketUtility.RegisterPacket<StringPacket>(1000);
        }
    }
}
