using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Exchange.Common;
using Exchange.DAL.Managers;

namespace Exchange.Server
{
    
    public delegate void ServerStartedHandler(IServerCore c);
    public delegate void ServerStoppedHandler(IServerCore c);

    public delegate void ClientConnectedHandler(Guid clientId);
    public delegate void ClientDisconnectedHandler(Guid clientId);

    public delegate void MessageReceivedHandler(Guid clientId, OrderItem orderItem);
    public delegate void MessageSubmittedHandler(Guid clientId, OrderItem orderItem);

    public class ServerCore : IServerCore
    {
        public static readonly ServerCore Instance = new ServerCore();
        private static bool _stop = false;

        public event ServerStartedHandler ServerStarted;
        public event ServerStoppedHandler ServerStopped;

        public event MessageReceivedHandler MessageReceived;
        public event MessageSubmittedHandler MessageSubmitted;

        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;

        public Guid Id { get; private set; }
        public DateTime StartTime { get; private set; } 

        private TcpListener _listener;
        private int _port;
        private IPAddress _ip;
        private string _cs;
        private List<Socket> _clients;
        public ServerCore()
        {
            Id = Guid.NewGuid();
        }
        public void Start(string ip, int port, string cs)
        {
            _cs = cs;
            _port = port;
            IPHostEntry host = Dns.GetHostEntry(ip);
            _ip = host.AddressList[0];
            StartTime = DateTime.Now;

            _listener = new TcpListener(_ip, port);
            _listener.Start();

            LogServerStarted();
            RiseServerStartedEvent();

            StartUpdatePrice();
            StartUpdateServerStatus();

            while (!_stop)
            {
                Socket socket = _listener.AcceptSocket();
                ThreadPool.QueueUserWorkItem(OnClientConnected, socket);
            }

            LogServerStopped();
            RiseServerStoppedEvent();
        }

        private void StartUpdateServerStatus()
        {
            var thread = new Thread(LogServerRunning);
            thread.Start();
        }

        private void StartUpdatePrice()
        {
            PriceManager.AutoUpdatePrices(_cs);
        }

        public void Stop()
        {
            _stop = true;
        }

        private void OnClientConnected(object state)
        {
            var clientId = Guid.Empty;
            try
            {
                using (var socket = (Socket)state)
                {
                    // ReSharper disable RedundantAssignment
                    var bytes = new byte[1024];
                    // ReSharper restore RedundantAssignment
                    var msg = new Message();
                    // ReSharper disable TooWideLocalVariableScope
                    OrderItem orderItem;
                    PriceItem priceItem;
                    // ReSharper restore TooWideLocalVariableScope

                    while (!_stop)
                    {
                        bytes = new byte[1024];
                        socket.Receive(bytes);
                        msg.Data = bytes;
                        orderItem = msg.Deserialize().ToOrder();
                        clientId = orderItem.ClientId;
                        RiseMessageReceivedEvent(orderItem.ClientId, orderItem);

                        switch (orderItem.CommandType)
                        {
                           case CommandTypeEnum.Exit:
                                socket.Close();
                                RiseClientDisconnectedEvent(clientId);
                                break;

                           case CommandTypeEnum.RequestPrice:
                                priceItem = GetPrice(orderItem.CurrencyType);
                                orderItem.Price = priceItem.Price;
                                orderItem.PriceId = priceItem.Id;
                                orderItem.CommandType = CommandTypeEnum.ResponsePrice;
                                break;
                         
                           case CommandTypeEnum.RequestOrder:
                                orderItem.Date = DateTime.Now;
                                SaveOrder(ref orderItem);
                                orderItem.CommandType = CommandTypeEnum.ResponseOrderSaved;
                                break;
                        }

                        msg = orderItem.Serialize();
                        socket.Send(msg.Data);

                        RiseMessageSubmittedeEvent(clientId,orderItem);
                    }
                }
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)
                {
                    RiseClientDisconnectedEvent(clientId);
                }
                else
                {
                    throw;
                }
            }
        }

        // Price Cash
        private List<PriceItem> _priceList = new List<PriceItem>();
        private DateTime? _updateTime;
        private PriceItem GetPrice(CurrencyTypeEnum currencyType)
        {
            lock (_priceList)
            {
                if (!_priceList.Any() || _updateTime == null || 
                    (_updateTime != null && _updateTime < DateTime.Now))
                {
                    var list = new List<PriceItem>();
                    while (!list.Any())
                    {
                        list = PriceManager.Get(_cs);
                        PriceManager.Update(_cs);
                    }
                    _priceList = list;
                    _updateTime = DateTime.Now.AddSeconds(10);
                }
                return _priceList.FirstOrDefault(x => x.CurrencyCode == currencyType);                
            }
        }

        private void SaveOrder(ref OrderItem orderItem)
        {
            OrderManager.Save(orderItem, _cs);          
        }

        private void LogServerStarted()
        {
            var serverItem = new ServerLogItem
            {
                ServerId = this.Id,
                Date = this.StartTime,
                EventType = EventTypeEnum.Started
            };
            ServerManager.Add(serverItem, _cs);            
        }
        public void LogServerStopped()
        {
            var serverItem = new ServerLogItem
            {
                AvailableClientsCount = GetAvailableClientsCount(),
                ServerId = this.Id,
                Date = this.StartTime,
                EventType = EventTypeEnum.Stopped
            };
            ServerManager.Add(serverItem, _cs);
        }
        public void LogServerRunning()
        {
            while (!_stop)
            {
                Thread.Sleep(5000);

                var serverItem = new ServerLogItem
                {
                    AvailableClientsCount = GetAvailableClientsCount(),
                    ServerId = this.Id,
                    Date = DateTime.Now,
                    EventType = EventTypeEnum.Running
                };
                ServerManager.Add(serverItem, _cs);
            }
        }
        private int GetAvailableClientsCount()
        {
            int available;
            int maxLimit;
            int maxThreads;
            ThreadPool.GetMaxThreads(out maxThreads, out maxLimit);
            ThreadPool.GetAvailableThreads(out available, out maxLimit);
            var running = maxThreads - available;
            return  running < 0 ? 0 : running;
        }

        public void RiseServerStoppedEvent()
        {
            var serverStopped = ServerStopped;

            if (serverStopped != null)
            {
                serverStopped(this);
            }
        }
        private void RiseServerStartedEvent()
        {
            var serverStarted = ServerStarted;

            if (serverStarted != null)
            {
                serverStarted(this);
            }
        }
        public void RiseClientDisconnectedEvent(Guid clientId)
        {
            var clientDisconnected = ClientDisconnected;

            if (clientDisconnected != null)
            {
                clientDisconnected(clientId);
            }
        }
        public void RiseClientConnectedEvent(Guid clientId)
        {
            var clientConnected = ClientConnected;

            if (clientConnected != null)
            {
                clientConnected(clientId);
            }
        }
        void RiseMessageReceivedEvent(Guid clientId, OrderItem orderItem)
        {
            var messageReceived = MessageReceived;

            if (messageReceived != null)
            {
                messageReceived(clientId, orderItem);
            }
        }
        void RiseMessageSubmittedeEvent(Guid clientId, OrderItem orderItem)
        {
            var messageSubmitted = MessageSubmitted;

            if (messageSubmitted != null)
            {
                messageSubmitted(clientId, orderItem);
            }
        }
        public void Dispose()
        {
            
            foreach (var client in this._clients)
            {
                client.Close();
            }
            
            _listener.Stop();
        }
    }
}
