using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace WebServer
{
    public class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].Tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.Clients[_toClient].Udp.SendData(_packet);
        }


        private static void SendTCPDataToAll(Packet _packet, int _roomID)
        {
            if (Server.Rooms.ContainsKey(_roomID))
            {
                _packet.WriteLength();

                foreach (var client in Server.Rooms[_roomID].Clients.Values)
                {
                    client.Tcp.SendData(_packet);
                }
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet, int _roomID, bool disconnect = false)
        {
            if (Server.Rooms.ContainsKey(_roomID))
            {
                _packet.WriteLength();
                foreach (var client in Server.Rooms[_roomID].Clients.Values)
                {
                    if (client.ID != _exceptClient)
                    {
                        client.Tcp.SendData(_packet);
                    }
                }
            }
            if (disconnect)
            {
                Server.RemoveClient(_exceptClient, _roomID); // tell the server to kill this client now that we are dced once wee sent this to everyone
            }
        }



        private static void SendUDPDataToAll(Packet _packet, int _roomID)
        {
            if (Server.Rooms.ContainsKey(_roomID))
            {
                _packet.WriteLength();
                foreach (var client in Server.Rooms[_roomID].Clients.Values)
                {
                    client.Udp.SendData(_packet);
                }
            }
        }
        private static void SendUDPDataToAll(int _exceptClient, int _roomID, Packet _packet)
        {
            if (Server.Rooms.ContainsKey(_roomID))
            {
                _packet.WriteLength();

                foreach (var client in Server.Rooms[_roomID].Clients.Values)
                {
                    if (client.ID != _exceptClient)
                    {
                        client.Udp.SendData(_packet);
                    }
                }
            }
        }
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendRooms(int _toClient, List<Room> _roomList)
        {
            foreach (var room in _roomList)
            {
                using (Packet _packet = new Packet((int)ServerPackets.sendRooms))
                {
                    _packet.Write(room.RoomID);
                    _packet.Write(room.RoomName);
                    _packet.Write(room.RoomSize);
                    _packet.Write(room.Clients.Count);
                    _packet.Write(_toClient);

                    SendTCPData(_toClient, _packet);
                }
            }
        }

        public static void RoomCreatedSuccesfully(int _toClient, int _roomID) // response to tell the client we made the room and they can proceed
        {
            Console.WriteLine("Room created");
            using (Packet _packet = new Packet((int)ServerPackets.roomCreated))
            {
                Console.WriteLine("RoomCreated " + _roomID);
                _packet.Write(_toClient);
                _packet.Write(_roomID);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void RoomJoinedSuccessfully(int _toClient, int _roomID)
        {
            Console.WriteLine("Room created");
            using (Packet _packet = new Packet((int)ServerPackets.roomJoined))
            {
                _packet.Write(_toClient);
                _packet.Write(_roomID);
                SendTCPData(_toClient, _packet);
            }
        }

        public static void RoomJoinFailed(int _toClient, int _roomID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.roomJoinFailed))
            {
                _packet.Write(_toClient);
                _packet.Write(_roomID);
                _packet.Write("Room Full");
                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendNewHost(int _toClient, int _roomID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.newHostSelected))
            {
                Console.WriteLine("telling user they are host");
                SendTCPData(_toClient, _packet);
            }
        }


        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.ID);
                _packet.Write(_player.Username);
                _packet.Write(_player.Position);
                _packet.Write(_player.Rotation);

                SendTCPData(_toClient, _packet);
            }
        }


        public static void SendMovement(int _exceptID, int _roomID, Vector3 _pos, Quaternion _rot)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerMovement))
            {
                _packet.Write(_exceptID);
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPDataToAll(_exceptID, _roomID, _packet);
            }
        }
        public static void SendObjectMovement(int _exceptID, int _roomID, int _netID, Vector3 _pos, Quaternion _rot) // sends the object movement to everyone except whom is using it
        {
            using (Packet _packet = new Packet((int)ServerPackets.objectMovement))
            {
                _packet.Write(_exceptID); // player id (guy who sent this request)
                _packet.Write(_netID); // net id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPDataToAll(_exceptID, _roomID, _packet);
            }
        }

        public static void SendObjectLocation(int _clientToSendToID, int _roomID, int _netID, Vector3 _pos, Quaternion _rot)
        {
            using (Packet _packet = new Packet((int)ServerPackets.setObjectLocation))
            {
                _packet.Write(_clientToSendToID); // player id (guy who sent this request)
                _packet.Write(_netID); // net id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendTCPData(_clientToSendToID, _packet); // sends as tcp because it NEEDS TO BE SET CORRECT
            }
        }


        public static void DisconnectClient(int _clientID, int _roomID, bool disconnect)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnect))
            {
                _packet.Write(_clientID); // player id (guy who sent this request)

                SendTCPDataToAll(_clientID, _packet, _roomID, disconnect);
            }
        }

    }
}