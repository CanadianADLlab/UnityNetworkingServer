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


        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.Clients.Count; i++)
            {
                Server.Clients[i].Tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet, bool disconnect = false)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.Clients.Count; i++)
            {
                if (i != _exceptClient)
                {
                    Server.Clients[i].Tcp.SendData(_packet);
                }
                if (disconnect)
                {
                    Console.WriteLine("Dcin " + i);
                }
            }
            if (disconnect)
            {
                Server.RemoveClient(_exceptClient); // tell the server to kill this client now that we are dced once wee sent this to everyone
            }
        }



        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.Clients.Count; i++)
            {
                Server.Clients[i].Udp.SendData(_packet);
            }
        }
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.Clients.Count; i++)
            {
                if (i != _exceptClient)
                {
                    Server.Clients[i].Udp.SendData(_packet);
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


        public static void SendMovement(int _exceptID, Vector3 _pos, Quaternion _rot)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerMovement))
            {
                _packet.Write(_exceptID);
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPDataToAll(_exceptID, _packet);
            }
        }
        public static void SendObjectMovement(int _exceptID, int _netID, Vector3 _pos, Quaternion _rot) // sends the object movement to everyone except whom is using it
        {
            using (Packet _packet = new Packet((int)ServerPackets.objectMovement))
            {
                _packet.Write(_exceptID); // player id (guy who sent this request)
                _packet.Write(_netID); // net id
                _packet.Write(_pos);
                _packet.Write(_rot);
                SendUDPDataToAll(_exceptID, _packet);
            }
        }

        public static void DisconnectClient(int _clientID, bool disconnect)
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnect))
            {
                _packet.Write(_clientID); // player id (guy who sent this request)

                SendTCPDataToAll(_clientID, _packet, disconnect);
            }
        }

    }
}