using System;
using System.Runtime.InteropServices;
using Exchange.Common;
using Exchange.Utils;

namespace Exchange.Client.Cmd
{
    class Program
    {
        private static string _ip;
        private static int _port;
        private static int _to;
        private static string _cs;

        static void Main(string[] args)
        {
            handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(handler, true);

            Console.Title = string.Format("Client {0}", ClientCore.Instance.Id);

            ClientCore.Instance.MessageReceived += Instance_MessageReceived;
            ClientCore.Instance.MessageSubmitted += Instance_MessageSubmitted;

            ClientCore.Instance.ClientConnected += Instance_ClientConnected;
            ClientCore.Instance.ClientDisconnected += Instance_ClientDisconnected;

            ClientCore.Instance.ReconnectWithError += Instance_ReconnectWithError;

            _cs = ConfigTools.Instance.Get<string>("connection-string");
            _ip = ConfigTools.Instance.Get<string>("host");
            _port = ConfigTools.Instance.Get<int>("port");
            _to = ConfigTools.Instance.Get<int>("request-timeout");

            ClientCore.Instance.Start(_ip, _port, _to, _cs);
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                ClientCore.Instance.LogExitClient();
                ClientCore.Instance.RiseClientDisConnectedEvent();
            }
            return false;
        }
        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        static void Instance_ReconnectWithError(IClientCore a, int errorCode)
        {
            if (errorCode == 10061)
            {
                Console.WriteLine("Client Error. Connection refused. No Server applicaiton running.");
                Console.WriteLine("Wait and try to reconnect after 5 sec..");
            }
            else if (errorCode == 10054)
            {
                Console.WriteLine("Client lost connection with Server");
                Console.WriteLine("Try to reconnect after 5 sec..");
            }            
        }

        static void Instance_ClientDisconnected(IClientCore a)
        {
            Console.WriteLine("Client {0} disconnected from server {1}:{2} at {3}",
                a.Id, _ip, _port, a.StartTime.ToString("G"));
        }

        static void Instance_ClientConnected(IClientCore a)
        {
            Console.WriteLine("Client {0} connected to server {1}:{2} at {3}", 
                a.Id, _ip, _port, a.StartTime.ToString("G"));
        }

        static void Instance_MessageSubmitted(IClientCore a, OrderItem orderItem)
        {
            switch (orderItem.CommandType)
            {
                case CommandTypeEnum.RequestPrice:
                    Console.WriteLine("Client Requested Price");
                    Console.WriteLine("CurrencyCode Type: {0}", orderItem.CurrencyType);
                    Console.WriteLine("======================================================");
                break;

                case CommandTypeEnum.RequestOrder:
                    Console.WriteLine("Client Request Order:");
                    Console.WriteLine("Command Type: {0}", orderItem.CommandType);
                    Console.WriteLine("CurrencyCode Type: {0}", orderItem.CurrencyType);
                    Console.WriteLine("Price: {0}", orderItem.Price);
                    Console.WriteLine("Value: {0}", orderItem.Value);
                    Console.WriteLine("======================================================");
                break;
            }
        }

        static void Instance_MessageReceived(IClientCore a, OrderItem orderItem)
        {
            switch (orderItem.CommandType)
            {
                case CommandTypeEnum.ResponsePrice:
                    Console.WriteLine("Currency Type: {0}", orderItem.CurrencyType);
                    Console.WriteLine("Server responsed Price: {0}", orderItem.Price);
                    Console.WriteLine("======================================================");
                    break;

                case CommandTypeEnum.ResponseOrderSaved:
                    Console.WriteLine("Server Saved Order:");
                    Console.WriteLine("Saved Order Id: {0}", orderItem.Id);
                    Console.WriteLine("CurrencyCode Type: {0}", orderItem.CurrencyType);
                    Console.WriteLine("Price: {0}", orderItem.Price);
                    Console.WriteLine("Value: {0}", orderItem.Value);
                    Console.WriteLine("======================================================");
                    break;
            }
        }
    }
}
