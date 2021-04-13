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

        public Room(int _RoomID, int _clientID, Client _firstClient, string _roomName, int _roomSize)
        {
            RoomID = _RoomID;
            RoomName = _roomName;
            RoomSize = _roomSize;
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