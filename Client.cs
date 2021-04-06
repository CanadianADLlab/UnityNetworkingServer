using System;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Client
    {
        public static int DataBufferSize = 4096;
        public int ID;

        public TCP Tcp;
        public Client(int _clientId)
        {
            ID = _clientId;
            Tcp = new TCP(ID);
        }


        public class TCP
        {
            public TcpClient Socket;
            private readonly int id;

            private NetworkStream stream;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                Socket = _socket;
                Socket.ReceiveBufferSize = DataBufferSize;
                Socket.SendBufferSize = DataBufferSize;

                stream = Socket.GetStream();
                receiveBuffer = new byte[DataBufferSize];
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
                Console.WriteLine("Sending welcome");
                ServerSend.Welcome(id,"Welcome to the server!");
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

                    stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallBack, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error receiving packet " + e);
                }
            }
        }
    }
}