using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Room
    {
        public Dictionary<int,Client> Clients = new Dictionary<int,Client>();
        public int RoomID ;
        public string RoomName;

        public int RoomSize = 10;


        public Room(int _RoomID,int _clientID,Client _firstClient,string _roomName)
        {
            OnPlayerJoinRoom(_firstClient);
            RoomID = _RoomID;
            RoomName = _roomName;
        }


        public void OnPlayerJoinRoom(Client _player)
        {
            Clients.Add(_player.ID,_player);
        }

         public void OnPlayerLeaveRoom(int id)
        {
            Clients.Remove(id);
            if(Clients.Count == 0) // no clients left 
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