using System;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public static class Constants
    {
        public const int TICK_PER_SEC = 30;
        public const int MS_PER_TICK = 1000/TICK_PER_SEC;
    }
}