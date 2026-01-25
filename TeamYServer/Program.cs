using TeamYServer.Networking;
using TeamYShared;
using TeamYShared.Networking;

namespace TeamYServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TeamY Server starting...");

            PacketProtocol.RegisterPackets();
            ServerPacketDispatcher dispatcher = new ServerPacketDispatcher();
            ServerEndpoint server = new ServerEndpoint(dispatcher, "TeamY", 14242);

            Console.WriteLine("Server running. Press CTRL+C to exit.");

            // Simple server loop
            while (true)
            {
                server.Poll();
                Thread.Sleep(10);
            }
        }
    }
}
