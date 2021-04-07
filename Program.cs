using System;
using System.Threading;

namespace WebServer
{
    class Program
    {

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

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            Server.Start(players, port);

        }

        private static void MainThread()
        {
            Console.WriteLine($"Thread has started at {Constants.TICK_PER_SEC} ticks");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
