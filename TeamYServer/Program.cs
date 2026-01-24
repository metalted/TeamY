using TeamYShared;

namespace TeamYServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TeamY Server starting...");

            int tick = 0;

            while (true)
            {
                tick++;
                Console.WriteLine($"Server tick {tick}");
                Thread.Sleep(1000);
            }
        }
    }
}
