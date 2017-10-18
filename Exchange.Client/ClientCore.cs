using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Exchange.Common;
using Exchange.DAL.Managers;

namespace Exchange.Client
{
    public delegate void ClientConnectedHandler(IClientCore a);
    public delegate void ClientDisConnectedHandler(IClientCore a);
    public delegate void ClientMessageReceivedHandler(IClientCore a, OrderItem orderItem);
    public delegate void ClientMessageSubmittedHandler(IClientCore a, OrderItem orderItem);
    public delegate void ReconnectWithErrorHandler(IClientCore a, int errorCode);
    

    public class ClientCore : IClientCore
    {
        public static readonly ClientCore Instance = new ClientCore();

        public event ClientConnectedHandler ClientConnected;
        public event ClientDisConnectedHandler ClientDisconnected;
        public event ClientMessageReceivedHandler MessageReceived;
        public event ClientMessageSubmittedHandler MessageSubmitted;
        public event ReconnectWithErrorHandler ReconnectWithError;

        public Guid Id { get; private set; }
        public DateTime StartTime { get; private set; }

        private string _cs;
        private int _port;
        private IPAddress _ip;
        private IPHostEntry _host;
        private int _timeout;

        private bool _stop;
        private readonly List<CurrencyTypeEnum> _clist;
        private readonly Random _r;
        public ClientCore()
        {
            Id = Guid.NewGuid();

            _clist = new List<CurrencyTypeEnum>
            {
                CurrencyTypeEnum.RUB,
                CurrencyTypeEnum.USD,
                CurrencyTypeEnum.EUR,
                CurrencyTypeEnum.CHF,
                CurrencyTypeEnum.JPY,
                CurrencyTypeEnum.GBP,
                CurrencyTypeEnum.CAD,
                CurrencyTypeEnum.CNY,
                CurrencyTypeEnum.BRL
            };

            _r = new Random(1);
        }

        public void RiseReconnectWithErrorHandler(int errorCode)
        {
            var reconnectWithError = ReconnectWithError;

            if (reconnectWithError != null)
            {
                reconnectWithError(this, errorCode);
            }
        }
        public void RiseMessageSubmittedHandler(OrderItem orderItem)
        {
            var messageSubmitted = MessageSubmitted;

            if (messageSubmitted != null)
            {
                messageSubmitted(this, orderItem);
            }
        }
        public void RiseMessageReceivedEvent(OrderItem orderItem)
        {
            var messageReceived = MessageReceived;

            if (messageReceived != null)
            {
                messageReceived(this, orderItem);
            }
        }
        public void RiseClientDisConnectedEvent()
        {
            var clientDisConnected = ClientDisconnected;

            if (clientDisConnected != null)
            {
                clientDisConnected(this);
            }
        }
        public void RiseClientConnectedEvent()
        {
            var clientConnected = ClientConnected;

            if (clientConnected != null)
            {
                clientConnected(this);
            }
        }

        public void Start(string ip, int port, int timeout, string cs)
        {
            _port = port;
            _host = Dns.GetHostEntry(ip);
            _ip = _host.AddressList[0];
            _timeout = timeout;
            _cs = cs;
            StartTime = DateTime.Now;
            RegisterClient();

            Run();
        }

        private void RegisterClient()
        {
            var clientItem = new ClientItem
            {
                Id = this.Id,
                StartDate = this.StartTime
            };

            ClientManager.Save(clientItem, _cs);
        }

        private void Run() 
        {
            try
            {
                var bytes = new byte[1024];
                var ipAddr = _host.AddressList[0];
                var ipEndPoint = new IPEndPoint(_ip, _port);


                using (var sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    sender.Connect(ipEndPoint);
                    RiseClientConnectedEvent();

                    // ReSharper disable TooWideLocalVariableScope
                    Message msg;
                    // ReSharper restore TooWideLocalVariableScope

                    var order = new OrderItem(Id);

                    while (!_stop)
                    {
                        Thread.Sleep(_timeout);

                        switch (order.CommandType)
                        {
                            case CommandTypeEnum.Exit:
                                order.CommandType = CommandTypeEnum.Exit;
                                RiseClientDisConnectedEvent();
                                _stop = true;
                                continue;

                            case CommandTypeEnum.None:
                                order.CommandType = CommandTypeEnum.RequestPrice;
                                order.CurrencyType = _clist[_r.Next(0, _clist.Count)];; 
                                break;

                            case CommandTypeEnum.ResponsePrice:
                                order.Value = _r.Next(1, 100000);
                                order.CommandType = CommandTypeEnum.RequestOrder;
                                break;

                            case CommandTypeEnum.ResponseOrderSaved:
                                order.Save();
                                order.Clear();
                                break;
                        }

                        msg = order.Serialize();
                        sender.Send(msg.Data);
                        RiseMessageSubmittedHandler(order);

                        sender.Receive(bytes);
                        msg.Data = bytes;
                        order = msg.Deserialize().ToOrder();
                        RiseMessageReceivedEvent(order);
                    }

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10061 || se.ErrorCode == 10054)
                {
                    RiseReconnectWithErrorHandler(se.ErrorCode);
                    Thread.Sleep(5000);
                    Run();
                }
                else
                {
                    throw;
                }
            }
        }

        public void LogExitClient()
        {
            var client = new ClientItem()
            {
                Id = this.Id,
                EndDate = DateTime.Now
            };
            ClientManager.Update(client, _cs);
        }


        public void Stop()
        {
            _stop = true;
        }
        public void Dispose()
        {
            // TODO: .. 
        }
    }
}
