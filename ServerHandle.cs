using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace WebServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIDCheck = _packet.ReadInt();
            string username = _packet.ReadString();
            Console.WriteLine($"{Server.Clients[_fromClient].Tcp.Socket.Client.RemoteEndPoint} connected succesfully and is now player {_fromClient}");

            if (_fromClient != _clientIDCheck)
            {
                Console.WriteLine($"Player {username} has gotten the wrong client id somehow!");
            }
            Server.Clients[_fromClient].SendIntoGame(username);
        }

        public static void PlayerMovementReceived(int _fromClient, Packet _packet)
        {
            int _id = _packet.ReadInt();
            Vector3 _pos = _packet.ReadVector3();
            Quaternion _rot = _packet.ReadQuaternion();

            Server.Clients[_fromClient].SendMovement(_id, _pos, _rot);
        }





    }
}