using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamYClient.Networking;
using TeamYShared.Networking;
using TeamYShared.Packets.Development;

namespace TeamYDevClient
{
    public class Program
    {
        private static ClientEndpoint _client;

        static void Main(string[] args)
        {
            Console.WriteLine("TeamY DevClient starting...");

            PacketProtocol.RegisterPackets();
            ClientPacketDispatcher dispatcher = new ClientPacketDispatcher();

            Thread.Sleep(2000);

            _client = new ClientEndpoint(dispatcher, appIdentifier: "TeamY");
            _client.Connect("127.0.0.1", 14242);

            Thread.Sleep(1000);

            _client.Send(new StringPacket
            {
                Data = "Hello from Client"
            });

            while (true)
            {
                _client?.Poll();

                Thread.Sleep(10);
            }
        }
    }
}
