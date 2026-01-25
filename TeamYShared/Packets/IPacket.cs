using LidgrenX.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared.Packets
{
    public interface IPacket
    {
        void Deserialize(NetIncomingMessage im);

        void Serialize(NetOutgoingMessage om);
    }
}
