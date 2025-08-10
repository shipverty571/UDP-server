using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDPModel
{
    public class Server
    {
        private const int Port = 8888;

        private UdpClient _server;

        private Thread _receiveMessagesThread;

        public Server()
        {
            _server = new UdpClient(Port);
            _receiveMessagesThread = new Thread(ReceiveMessages);
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            IsRunning = true;
            _receiveMessagesThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _server.Close();
            _receiveMessagesThread.Join();
        }

        private void ReceiveMessages()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (IsRunning)
                {
                    byte[] data = _server.Receive(ref sender);
                    // Уйдет в класс-обработчик ввода
                    string message = Encoding.UTF8.GetString(data);

                    Console.WriteLine($"\n[{sender}] {message}");
                }
            }

            // Подумать
            catch (SocketException) { }
            catch (ObjectDisposedException) { }
        }
    }
}