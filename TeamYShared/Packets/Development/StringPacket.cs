using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Packets.Development
{
    public struct StringPacket : IPacket
    {
        public string Data;

        public void Deserialize(NetIncomingMessage im)
        {
            Data = im.ReadString();
        }

        public void Serialize(NetOutgoingMessage om)
        {
            om.Write(Data);
        }
    }
}
