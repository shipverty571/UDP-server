using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UDPModel.Exceptions;
using UDPModel.Repositories;

namespace UDPModel
{
    public class Server
    {
        private const int Port = 8888;

        private const int GetMetricsTimeout = 5000;

        private UdpClient _server;

        private Thread _receiveMessagesThread;

        private Thread _getMetricsThread;

        private MetricRepository _repository;

        public Action<string> OnInformation;

        public Server()
        {
            _server = new UdpClient(Port);
            _receiveMessagesThread = new Thread(ReceiveMessages);
            _getMetricsThread = new Thread(GetMetrics);

            _repository = new MetricRepository();
        }

        public bool IsRunning { get; private set; }

        public void Start()
        {
            IsRunning = true;
            _receiveMessagesThread.Start();
            _getMetricsThread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _server.Close();
            _receiveMessagesThread.Join();
            _getMetricsThread.Join();
        }

        private void ReceiveMessages()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (IsRunning)
                {
                    var data = _server.Receive(ref sender);
                    var message = Encoding.UTF8.GetString(data);

                    // возможно придется вынести в отдельный метод
                    var isValidMessage = IsValidMessage(message);
                    if (isValidMessage)
                    {
                        var metric = MetricParser.Parse(message);
                        _repository.Add(metric.Name, metric.Value);
                    }
                }
            }

            // Подумать
            catch (SocketException socketException)
            {
                OnInformation?.Invoke($"Ошибка:{socketException}");
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                OnInformation?.Invoke($"Ошибка:{objectDisposedException}");
            }
        }

        private bool IsValidMessage(string message)
        {
            try
            {
                MessageValidator.IsValidMessage(message);

                return true;
            }
            catch (NotValidMessageException exception)
            {
                OnInformation?.Invoke(exception.Message);

                return false;
            }
        }

        private void GetMetrics()
        {
            while (IsRunning)
            {
                Thread.Sleep(GetMetricsTimeout);

                var metrics = _repository.GetAll();
                OnInformation?.Invoke(metrics);
            }
        }
    }
}