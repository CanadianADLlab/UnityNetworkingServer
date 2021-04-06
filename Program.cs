using System;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "Game Server";
            Console.WriteLine("Enter total players");
            int players = -999;
            int port = -999;
            while (players <= 0)
            {
                try
                {
                    players = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Enter valid total players");
                }
            }
            Console.WriteLine("Enter Port to run on");
            while (port <= 0)
            {
                try
                {
                    port = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Enter valid port");
                }
            }
            Server.Start(players,port);
            Console.ReadKey();
        }
    }
}
