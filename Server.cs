using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Server
    {
        public static int Port { get; private set; }

        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        public static Dictionary<int, Room> Rooms = new Dictionary<int, Room>();
        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static int clientIDCounter = 0;
        private static int roomIDCounter = 0;

        public static void Start(int _portNumber)
        {
            Console.WriteLine("Starting Server....");
            Port = _portNumber;
            InitializeServerData();
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();

            tcpListener.BeginAcceptTcpClient(new System.AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);

            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine("Server has started");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new System.AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine("Connection comming from " + _client.Client.RemoteEndPoint + " ...");

            clientIDCounter++;
            Clients.Add(clientIDCounter, new Client(clientIDCounter));
            Clients[clientIDCounter].Tcp.Connect(_client);
            return;

        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (Clients[_clientId].Udp.EndPoint == null)
                    {
                        Clients[_clientId].Udp.Connect(_clientEndPoint);
                        return;
                    }


                    if (Clients[_clientId].Udp.EndPoint.ToString() == _clientEndPoint.ToString())
                    {
                        Clients[_clientId].Udp.HandleData(_packet);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public static void AddRoom(string _roomName, int _firstClientID,int _roomSize)
        {
            roomIDCounter++;
            var newRoom = new Room(roomIDCounter, _firstClientID, Clients[_firstClientID], _roomName,_roomSize);
            Rooms.Add(roomIDCounter, newRoom);

            ServerSend.RoomCreatedSuccesfully(_firstClientID, roomIDCounter);
            ServerSend.SendNewHost(_firstClientID, roomIDCounter);

        }
        public static void AddClientToRoom(int _clientID, int _roomID)
        {

            if (Rooms[_roomID].OnPlayerJoinRoom(Server.Clients[_clientID]))
            {
                ServerSend.RoomJoinedSuccessfully(_clientID, _roomID);
            }
            else
            {
                 ServerSend.RoomJoinFailed(_clientID, _roomID);
            }
        }

        public static void RemoveClient(int _clientID, int _roomID) // removes and disconnects a client
        {
            if (_roomID >= 0)
            {
                Console.WriteLine("Removing this asshole from the room " + _roomID);
                if (Rooms[_roomID].Clients.ContainsKey(_clientID))
                {
                    Rooms[_roomID].OnPlayerLeaveRoom(_clientID);
                }

            }
            if (Clients.ContainsKey(_clientID))
            {
                Clients[_clientID].Tcp.Disconnect();
                Clients[_clientID].Udp.Disconnect();
                Clients.Remove(_clientID);
            }

        }
        public static void RemoveRoom(int _roomID)
        {
            Console.WriteLine("Trashing the room my guy");
            Rooms.Remove(_roomID);
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static void InitializeServerData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
             {
            {(int)ServerPackets.welcome,ServerHandle.WelcomeReceived},
            {(int)ClientPackets.playerMovement,ServerHandle.PlayerMovementReceived},
            {(int)ClientPackets.objectMovement,ServerHandle.ObjectMovementReceived},
            {(int)ClientPackets.playerDisconnect,ServerHandle.DisconnectPlayer},
            {(int)ClientPackets.createRoom,ServerHandle.CreateRoom},
            {(int)ClientPackets.levelLoaded,ServerHandle.LevelLoaded},
            {(int)ClientPackets.joinRoom,ServerHandle.JoinRoom},
            {(int)ClientPackets.getRooms,ServerHandle.SendRooms},
            {(int)ClientPackets.objectLocationSet,ServerHandle.SetObjectLocation},
            };
            Console.WriteLine("inited");
        }

    }
}