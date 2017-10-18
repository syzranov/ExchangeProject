using System;
using System.Runtime.InteropServices;
using Exchange.Common;
using Exchange.Utils;

namespace Exchange.Server.Cmd
{
    class Program
    {
        private static string _ip;
        private static int _port;
        private static string _cs;
        static void Main(string[] args)
        {
            handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(handler, true);

            Console.Title = string.Format("Server-{0}", ServerCore.Instance.Id);
            ServerCore.Instance.ServerStarted += Instance_ServerStarted;
            ServerCore.Instance.ServerStopped += Instance_ServerStopped;

            ServerCore.Instance.ClientDisconnected += Instance_ClientDisconnected;
            ServerCore.Instance.ClientConnected += Instance_ClientConnected;
            
            ServerCore.Instance.MessageSubmitted += Instance_MessageSubmitted;

            _ip = ConfigTools.Instance.Get<string>("exchange-server-host");
            _port = ConfigTools.Instance.Get<int>("exchange-server-port");
            _cs = ConfigTools.Instance.Get<string>("connection-string");

            ServerCore.Instance.Start(_ip, _port, _cs);
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                ServerCore.Instance.LogServerStopped();
                ServerCore.Instance.RiseServerStoppedEvent();
            }
            return false;
        }
        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static void Instance_ServerStopped(IServerCore c)
        {
            Console.WriteLine("Server {0} stopped", c.Id);
        }

        static void Instance_MessageSubmitted(Guid clientId, OrderItem orderItem)
        {
            switch (orderItem.CommandType)
            {
                case CommandTypeEnum.ResponsePrice:
                    Console.WriteLine("Price requested");
                    Console.WriteLine("CurrencyType: {0}", orderItem.CurrencyType);
                    Console.WriteLine("Price: {0}", orderItem.Price);
                    Console.WriteLine("Price sent to client");
                    Console.WriteLine("======================================================");
                    break;

                case CommandTypeEnum.ResponseOrderSaved:
                    Console.WriteLine("Order Saved");
                    Console.WriteLine("Order Id: {0}", orderItem.Id);
                    Console.WriteLine("======================================================");
                    break;

                case CommandTypeEnum.ResponseOrderCanceled:
                    Console.WriteLine("Order Canceled");
                    Console.WriteLine("======================================================");
                    break;
            }
        }

        static void Instance_ClientConnected(Guid clientId)
        {
            Console.WriteLine("Client {0} connected.", clientId);            
        }
        static void Instance_ClientDisconnected(Guid clientId)
        {
            Console.WriteLine("Client {0} disconnected.", clientId);
        }

        static void Instance_ServerStarted(IServerCore c)
        {
            Console.WriteLine("Server running...");
            Console.WriteLine("Waits for connections on {0}:{1}", _ip, _port);
        }
    }
}
