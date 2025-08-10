using System;
using UDPModel;

namespace UDP_Server
{
    internal class Program
    {
        private static Server _server;

        static Program()
        {
            _server = new Server();
            _server.OnInformation = Console.WriteLine;
        }

        public static void Main(string[] args)
        {
            _server.Start();
            Console.WriteLine("Сервер запущен");

            // в основном потоке слушаем ввод - если enter нажали, то все вырубаем
            while (_server.IsRunning)
            {
                string message = Console.ReadLine();

                if (string.IsNullOrEmpty(message))
                {
                    Console.WriteLine("Остановка сервера...");
                    _server.Stop();
                    break;
                }
            }

            Console.WriteLine("Сервер остановлен");
        }
    }
}