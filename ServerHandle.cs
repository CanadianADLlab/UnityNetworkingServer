using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Linq;

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
            //    Server.Clients[_fromClient].SendIntoGame(username);
            SendRooms(_fromClient, _packet);

        }

        public static void SendRooms(int _fromClient, Packet _packet)
        {
            var roomList = Server.Rooms.Values.ToList(); // throws error without System.Linq
            Server.Clients[_fromClient].SendRooms(roomList);
        }

        public static void LevelLoaded(int _fromClient, Packet _packet)
        {
            int _clientIDCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            int _roomID = _packet.ReadInt();
            bool _isVR = _packet.ReadBool();
            Server.Clients[_fromClient].SendIntoGame(_username, _roomID,_isVR);
        }
        public static void DisconnectPlayer(int _fromClient, Packet _packet)
        {
            int _clientIDToRemove = _packet.ReadInt();
            int _roomID = _packet.ReadInt();
            Console.WriteLine($"{Server.Clients[_fromClient].Tcp.Socket.Client.RemoteEndPoint} player has disconnected with the id of {_fromClient}");

            ServerSend.DisconnectClient(_clientIDToRemove, _roomID, true); // true is a disconnect bool ( tells the server to remove the client and close connections)
                                                                           // Server.RemoveClient(_clientIDToRemove);
        }

        public static void CreateRoom(int _fromClient, Packet _packet)
        {
            Console.WriteLine("Creating room");
            int _id = _packet.ReadInt();
            string _roomName = _packet.ReadString();
            int _roomSize = _packet.ReadInt();

            Server.AddRoom(_roomName, _id, _roomSize);
            // Server.Clients[_fromClient].SendMovement(_id, _pos, _rot);
        }

        public static void JoinRoom(int _fromClient, Packet _packet)
        {
            Console.WriteLine("Creating room");
            int _id = _packet.ReadInt();
            int _roomID = _packet.ReadInt();

            //  Server.AddRoom(_roomName, _id);
            Server.AddClientToRoom(_id, _roomID);
        }
        public static void PlayerMovementReceived(int _fromClient, Packet _packet)
        {
            int _id = _packet.ReadInt();
            int _roomID = _packet.ReadInt();
            Vector3 _pos = _packet.ReadVector3();
            Quaternion _rot = _packet.ReadQuaternion();
            bool _lerp = _packet.ReadBool();

            Server.Clients[_fromClient].SendMovement(_id, _roomID, _pos, _rot,_lerp);
        }




        public static void ObjectMovementReceived(int _fromClient, Packet _packet)
        {
            int _id = _packet.ReadInt();
            int _roomID = _packet.ReadInt();
            int _netID = _packet.ReadInt(); // unique id for the object
            Vector3 _pos = _packet.ReadVector3();
            Quaternion _rot = _packet.ReadQuaternion();

            Server.Clients[_fromClient].SendObjectMovement(_id, _roomID, _netID, _pos, _rot);
        }

       public static void SetObjectLocation(int _fromClient, Packet _packet)
       {
            int _clientToSendID = _packet.ReadInt();
            int _roomID = _packet.ReadInt();
            int _netID = _packet.ReadInt(); // unique id for the object
            Vector3 _pos = _packet.ReadVector3();
            Quaternion _rot = _packet.ReadQuaternion();

            ServerSend.SendObjectLocation(_clientToSendID, _roomID, _netID, _pos, _rot);
       }

    }
}