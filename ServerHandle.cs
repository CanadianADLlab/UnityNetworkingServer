using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIDCheck = _packet.ReadInt();
            string username = _packet.ReadString();
            Console.WriteLine($"{Server.clients[_fromClient].Tcp.Socket.Client.RemoteEndPoint} connected succesfully and is now player {_fromClient}");

            if (_fromClient != _clientIDCheck)
            {
                Console.WriteLine($"Player {username} has gotten the wrong client id somehow!");
            }
        }

    }
}