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
            private Packet receivedData;

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
                            Server.packetHandlers[_packetId](id,_packet);
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
            catch (Exception e)
            {
                Console.WriteLine("Error receiving packet " + e);
            }
        }
    }
}
}