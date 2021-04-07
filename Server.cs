using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;


        public static void Start(int _maxPlayers, int _portNumber)
        {
            Console.WriteLine("Starting Server....");
            MaxPlayers = _maxPlayers;
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

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].Tcp.Socket == null)
                {
                    clients[i].Tcp.Connect(_client);
                    return;
                }
            }
            Console.WriteLine("Server is full");
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

                    if (clients[_clientId].Udp.EndPoint == null)
                    {
                        clients[_clientId].Udp.Connect(_clientEndPoint);
                        return;
                    }



                    if (clients[_clientId].Udp.EndPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].Udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if(_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(),_packet.Length(),_clientEndPoint,null,null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
             {
            {(int)ServerPackets.welcome,ServerHandle.WelcomeReceived},
            {(int)ServerPackets.udpTest,ServerHandle.UDPReceived}
            };
            Console.WriteLine("inited");
        }

    }
}