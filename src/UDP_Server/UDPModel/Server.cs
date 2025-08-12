using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UDPModel.Repositories;
using UDPModel.Utilities;

namespace UDPModel
{
    public class Server
    {
        private const int GetMetricsTimeout = 5000;

        private UdpClient _server;

        private Thread _receiveMessagesThread;

        private Thread _getMetricsThread;

        private MetricRepository _repository;

        public Action<string> OnInformation;

        public Server(int port)
        {
            _server = new UdpClient(port);
            _server.Client.ReceiveTimeout = 1000;
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
            _receiveMessagesThread.Join();
            _getMetricsThread.Join();
        }

        private void ReceiveMessages()
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (IsRunning)
            {
                try
                {
                    var data = _server.Receive(ref sender);
                    var message = Encoding.UTF8.GetString(data);

                    var isValidMessage = MessageValidator.IsValidMessage(message);
                    if (isValidMessage)
                    {
                        var metric = MetricParser.Parse(message);
                        _repository.Add(metric.Name, metric.Value);
                    }
                    else
                    {
                        OnInformation?.Invoke($"Ошибка формата: {message}");
                    }
                }
                catch (SocketException socketExceptionTimeout) when
                    (socketExceptionTimeout.SocketErrorCode == SocketError.TimedOut)
                {
                    // Если нет данных за 1 секунду, то флаг проверяется и работа продолжается
                }
                catch (SocketException socketException)
                {
                    OnInformation?.Invoke($"Ошибка:{socketException}");
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    OnInformation?.Invoke($"Ошибка:{objectDisposedException}");
                }
                catch (FormatException formatException)
                {
                    OnInformation?.Invoke($"Ошибка:{formatException}");
                }
            }

            _server.Close();
        }

        private void GetMetrics()
        {
            while (IsRunning)
            {
                Thread.Sleep(GetMetricsTimeout);

                var metrics = _repository.GetAll();
                var text = string.Empty;
                if (metrics.Count == 0)
                {
                    text = "[METRIC] Нет данных";
                }
                else
                {
                    text =
                        "[METRIC] " +
                        string.Join(" | ", metrics.Select(kv => $"{kv.Key} = {kv.Value}"));
                }

                OnInformation?.Invoke(text);
            }
        }
    }
}