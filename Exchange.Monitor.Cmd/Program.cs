using System;
using System.Globalization;
using Exchange.Utils;

namespace Exchange.Monitor.Cmd
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = 
                string.Format("Monitor instance {0}", Monitor.Instance.Id);

            Monitor.Instance.TickPeriod =
                ConfigTools.Instance.Get<int>("refresh-period");

            Monitor.Instance.ConnectionString = 
                ConfigTools.Instance.Get<string>("connection-string");

            Monitor.Instance.StartNotificate += monitor_StartNotificate;
            Monitor.Instance.StopNotificate += monitor_StopNotificate;
            Monitor.Instance.StatisticUpdated += monitor_StatisticUpdated;

            Monitor.Instance.StartMonitor();
         
        }

        static void monitor_StatisticUpdated(object o)
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("========================================================");
            Console.WriteLine(" EXCHANGE RATES STATISTIC\t\t\t\t");
            Console.WriteLine(" Refresh period {0} sec\t\t\t\t\t", 
                (Monitor.Instance.TickPeriod / 1000));
            Console.WriteLine("========================================================");
            Console.WriteLine(" {0}\t{1}\t{2}\t{3}\t{4}\t\t\t",
                "Num.","Curr.","Price", "Number", "Average");
            Console.WriteLine(" {0}\t{1}\t{2}\t{3}\t{4}\t\t\t",
                "","Name","", "Of", "Tran.");
            Console.WriteLine(" {0}\t{1}\t{2}\t{3}\t{4}\t\t\t",
                "","", "","Tran.", "Value");
            Console.WriteLine("========================================================");

            var i = 0;
            foreach (var item in Monitor.Instance.Statistic)
            {
                i++;
                switch (i % 2)
                {
                    case 0:
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine(" {0}\t{1}\t{2}\t{3}\t{4}\t\t",
                    i, 
                    item.Name, 
                     
                    (decimal.Round(item.Price,2) == (decimal)0.0 ? "0.00" :
                    decimal.Round(item.Price,2).ToString(CultureInfo.InvariantCulture)),

                    item.DealCount,

                    (decimal.Round(item.MiddleValue, 2) == (decimal)0.0 ? "0.00\t" :
                    decimal.Round(item.MiddleValue, 2).ToString(CultureInfo.InvariantCulture)));
            }

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.WriteLine("========================================================");
            Console.WriteLine(" Last Update time: {0}\t\t\t\t", 
                DateTime.Now.ToString("hh:mm:ss"));

            Console.WriteLine(" Server Name: {0}\t",
                (string.IsNullOrEmpty(Monitor.Instance.ServerName)
                ? "Not Available\t\t\t" 
                : Monitor.Instance.ServerName));

            Console.WriteLine(" Connected clients: {0}\t\t\t\t\t", 
                Monitor.Instance.ConnectectedClientsCount);

            Console.WriteLine(" Total Orders Count: {0}\t\t\t\t", 
                Monitor.Instance.TotalOrdersCount == 0
                ? "0\t"
                : Monitor.Instance.TotalOrdersCount.ToString(CultureInfo.InvariantCulture));

            Console.WriteLine(" Total Orders Value: {0}\t\t",
                decimal.Round(Monitor.Instance.TotalOrdersValue, 2)
                == (decimal)0.0 ? "0.00\t\t" :
                    decimal.Round(Monitor.Instance.TotalOrdersValue, 2)
                    .ToString(CultureInfo.InvariantCulture) + "\t");

            Console.WriteLine(" Server Status: {0}\t\t\t\t\t",
                Monitor.Instance.ServerStatus ? "Working" : "Stopped");

            Console.WriteLine(" Server Working Time: {0}\t\t\t\t", 
                Monitor.Instance.ServerTimeWorking);

            Console.WriteLine(" \t\t\t\t\t\t\t");
            Console.ResetColor();
        }

        static void monitor_StopNotificate()
        {
            Console.WriteLine("Monitoring Stopped.");
        }

        static void monitor_StartNotificate()
        {
            Console.WriteLine("Monitoring Starting..");
        }
    }
}
