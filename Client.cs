using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace WebServer
{
    public class Client
    {
        public static int DataBufferSize = 4096;
        public int ID;

        public int RoomID;
        public Player Player;
        public TCP Tcp;
        public UDP Udp;
        public Client(int _clientId)
        {
            ID = _clientId;
            Tcp = new TCP(ID);
            Udp = new UDP(ID);
        }


        public class TCP
        {
            public TcpClient Socket;
            private readonly int id;

            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receivedData;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Disconnect()
            {
                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }
                if (stream != null)
                {
                    stream.Close(); // close the stream from this
                    stream = null;
                }
            }
            public void Connect(TcpClient _socket)
            {
                Socket = _socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                stream = Socket.GetStream();
                receiveBuffer = new byte[DataBufferSize];
                receivedData = new Packet();
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
                Console.WriteLine("Sending welcome");
                ServerSend.Welcome(id, "Welcome to the server!");
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);
                if (receivedData.UnreadLength() >= 4) // means an id was sent because an int contains 4 bytes
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });
                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4) // means an id was sent because an int contains 4 bytes
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }
                return false;
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error sending data to player " + e);
                }


            }
            private void ReceiveCallBack(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        return;
                    }
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));

                    stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
                }
                catch
                {
                    Disconnect(); // there is a chance that an exception maaay get thrown and if soo we just disconnect (happens during disconnect)
                }
            }
        }
        public class UDP
        {
            public IPEndPoint EndPoint;
            private int id;


            public UDP(int _id)
            {
                id = _id;
            }
            public void Disconnect()
            {
                EndPoint = null; // there might be a way to dc a udp im not sure but for now fuck it
            }
            public void Connect(IPEndPoint _endPoint)
            {
                EndPoint = _endPoint;
            }
            public void SendData(Packet _packet)
            {
                Server.SendUDPData(EndPoint, _packet);
            }



            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }
        }
        public void SendMovement(int _id, int _roomID, Vector3 _pos, Quaternion _rot, bool _lerp)
        {
            ServerSend.SendMovement(_id, _roomID, _pos, _rot, _lerp); // tell the game server to spawn the other pla yer
        }

        public void SendObjectMovement(int _id, int _roomID, int _netID, Vector3 _pos, Quaternion _rot)
        {
            ServerSend.SendObjectMovement(_id, _roomID, _netID, _pos, _rot); // tell the game server to spawn the other pla yer
        }

        public void SendRooms(List<Room> _roomList)
        {
            Console.WriteLine("Sending rooms too " + ID);
            ServerSend.SendRooms(ID, _roomList); // tell the game server to spawn the other player
        }

        public void SendIntoGame(string _playerName, int _roomID)
        {
            Player = new Player(ID, _roomID, _playerName, Vector3.Zero);
            Console.WriteLine("The room is  " + _roomID);
            foreach (Client _client in Server.Rooms[_roomID].Clients.Values)
            {
                if (_client.Player != null)
                {
                    if (_client.ID != ID)
                    {
                        ServerSend.SpawnPlayer(ID, _client.Player); // tell the game server to spawn the other pla yer
                    }
                }
            }

            foreach (Client _client in Server.Rooms[_roomID].Clients.Values)
            {
                if (_client.Player != null)
                {
                    ServerSend.SpawnPlayer(_client.ID, Player); // tell the player to spawn me
                }
            }
        }
    }
}