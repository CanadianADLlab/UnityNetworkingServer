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
        private static TcpListener TcpListener;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;


        public static void Start(int _maxPlayers, int _portNumber)
        {
            Console.WriteLine("Starting Server....");
            MaxPlayers = _maxPlayers;
            Port = _portNumber;
            InitializeServerData();
            TcpListener = new TcpListener(IPAddress.Any, Port);
            TcpListener.Start();

            TcpListener.BeginAcceptTcpClient(new System.AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine("Server has started");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = TcpListener.EndAcceptTcpClient(_result);
            TcpListener.BeginAcceptTcpClient(new System.AsyncCallback(TCPConnectCallback), null);
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

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
             {
            {(int)ServerPackets.welcome,ServerHandle.WelcomeReceived}
            };
            Console.WriteLine("inited");
        }

    }
}