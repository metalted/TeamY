using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamYShared
{
    public interface IPacket
    {
        void Deserialize(NetIncomingMessage im);

        void Serialize(NetOutgoingMessage om);
    }
}
