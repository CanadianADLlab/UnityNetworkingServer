using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Room
    {
        public List<Client> Clients = new List<Client>();
        public int RoomID ;
        public string RoomName;

        public int RoomSize = 10;


        public Room(int _RoomID,Client _firstClient,string _roomName)
        {
            Clients.Add(_firstClient);
            RoomID = _RoomID;
            RoomName = _roomName;
        }


        public void OnPlayerJoinRoom(Client _player)
        {
            Clients.Add(_player);
        }

         public void OnPlayerLeaveRoom(Client _player)
        {
            Clients.Remove(_player);
        }


        public void OnRoomClosed()
        {
            Clients.Clear();
        }
    }
}