using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UDP_Server
{
    internal class Program
    {
        private const int Port = 8888;

        private static bool _isRunning;

        private static UdpClient _server;

        private static Thread _receiveMessagesThread;

        public Program()
        {
            _receiveMessagesThread = new Thread(ReceiveMessages);
        }

        public static void Main(string[] args)
        {
            StartServer();

            // в основном потоке слушаем ввод - если enter нажали, то все вырубаем
            while (_isRunning)
            {
                string message = Console.ReadLine();

                if (string.IsNullOrEmpty(message))
                {
                    StopServer();
                    break;
                }
            }
        }

        private static void ReceiveMessages()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (_isRunning)
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

        private static void StartServer()
        {
            _server = new UdpClient(Port);
            _isRunning = true;

            // создаем и запускаем прослушку в отдельном потоке
            _receiveMessagesThread.Start();

            // дожидаеся завершения потока
            _receiveMessagesThread.Join();
        }

        private static void StopServer()
        {
            _isRunning = false;
            _server.Close();
        }
    }
}