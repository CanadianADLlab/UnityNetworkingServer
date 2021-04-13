using System;
using System.Threading;

namespace WebServer
{
    class Program
    {
        public static int ticksPerSec;
        public static int msPerTick;
        private static bool isRunning = false;
        static void Main(string[] args)
        {

            Console.Title = "Game Server";
            Console.WriteLine("Enter total players");
            isRunning = true;
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
            Console.WriteLine("Enter ticks per second");
            while (ticksPerSec <= 0)
            {
                try
                {
                    ticksPerSec = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Enter valid ticks per second");
                }
            }
            msPerTick = 1000 / ticksPerSec;
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            Server.Start(players, port);

        }

        private static void MainThread()
        {
            Console.WriteLine($"Thread has started at {ticksPerSec} ticks");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(msPerTick);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
