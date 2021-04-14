using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Room
    {
        public Dictionary<int, Client> Clients = new Dictionary<int, Client>();
        public int RoomID;
        public string RoomName;

        public int RoomSize;

        public int RoomHostID; // the id of the person who created or is last in the room

        public Room(int _RoomID, int _clientID, Client _firstClient, string _roomName, int _roomSize)
        {
            RoomID = _RoomID;
            RoomName = _roomName;
            RoomSize = _roomSize;
            RoomHostID = _firstClient.ID;
            OnPlayerJoinRoom(_firstClient);
        }


        public bool OnPlayerJoinRoom(Client _player)
        {
            if (Clients.Count < RoomSize)
            {
                Clients.Add(_player.ID, _player);
                return true;
            }
            return false;
        }

        public void OnPlayerLeaveRoom(int id)
        {
            if (Clients[id].ID == RoomHostID && Clients.Count != 0) // host quit so we need to set a new one 
            {
                foreach (var Client in Clients.Values)
                {
                    RoomHostID = Client.ID; // Setting the new host to the first client we find 
                    ServerSend.SendNewHost(RoomHostID, RoomID); // we need to hit our client up and be like EH YOU HOST NOW
                    break;
                }
            }
            Clients.Remove(id);
            if (Clients.Count == 0) // no clients left 
            {
                Server.RemoveRoom(RoomID); // Rooms empty so lets trash that boi
            }
        }


        public void OnRoomClosed()
        {
            Clients.Clear();
        }
    }
}