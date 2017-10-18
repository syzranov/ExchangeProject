using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Exchange.Common;
using Exchange.DAL.Managers;
using Exchange.Extentions;

namespace Exchange.Monitor
{
    public delegate void StatisticUpdatedHandler(object o);
    public delegate void StartNotificateHandler();
    public delegate void StopNotificateHandler();

    public class Monitor : IMonitor
    {
        public static readonly Monitor Instance = new Monitor();

        public Monitor()
        {
            Id = Guid.NewGuid();
            Statistic = new List<StatItem>();
            StatServer = new StatServerItem();
        }
        public Guid Id { get; private set; }

        public void Dispose()
        {
            // TODO: ...
        }

        public event StatisticUpdatedHandler StatisticUpdated;
        public event StartNotificateHandler StartNotificate;
        public event StopNotificateHandler StopNotificate;

        private string _cs;
        private int _port;
        private IPAddress _ip;
        private IPHostEntry _host;
        private int _timeout;
        public void StartMonitor()
        {
            Run();
            RiseStartMonitorEvent();

            var task = new Thread(Run);
            task.Start();
        }

        public void StopMonitor()
        {
            RiseStopMonitorEvent();
        }
        void RiseShowValueAndPriceEvent()
        {
            var statisticUpdated = this.StatisticUpdated;

            if (statisticUpdated != null)
            {
                statisticUpdated(this);
            }
        }
        void RiseStartMonitorEvent()
        {
            var startNotificate = this.StartNotificate;

            if (startNotificate != null)
            {
                startNotificate();
            }            
        }
        void RiseStopMonitorEvent()
        {
            var stopNotificate = this.StopNotificate;

            if (stopNotificate != null)
            {
                stopNotificate();
            }
        }

        void Run()
        {
            var tmp = new List<StatItem>();
            var r = new Random(1);
            while (true)
            {
                tmp.Clear();
                Statistic.Clear();
                tmp = MonitorManager.GetCurrencyStatictic(TickPeriod.ToInt(), ConnectionString);

                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.RUB));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.USD));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.EUR));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.CHF));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.JPY));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.GBP));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.CAD));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.CNY));
                Statistic.Add(GetByCurr(tmp, CurrencyTypeEnum.BRL));

                ClearStatServer();

                StatServer = MonitorManager.GetServerStatictic(TickPeriod.ToInt(), ConnectionString);

                RiseShowValueAndPriceEvent();
                Thread.Sleep(TickPeriod.ToInt());
            }
        }

        private StatItem GetByCurr(List<StatItem> list, CurrencyTypeEnum ct)
        {
            lock (list)
            {
                var item = new StatItem();
                item.CurrencyType = ct;
                item.Name = ct.ToString();

                if (list.Any(x => x.CurrencyType == ct))
                {
                    var d = list.First(x => x.CurrencyType == ct);
                    item.DealCount = d.DealCount;
                    item.PriceId = d.PriceId;
                    item.Price = d.Price;
                    item.MiddleValue = d.MiddleValue;
                    item.Name = d.Name;
                }
                else
                {
                    item.DealCount = 0;
                    item.PriceId = 0;
                    item.Price = (decimal)0.0;
                    item.MiddleValue = (decimal)0.0;
                }
                return item;
            }
        }

        private void ClearStatServer()
        {
            StatServer.ClientCount = 0;
            StatServer.DealCount = 0;
            StatServer.DealValue = 0;
            StatServer.IsServerAvailable = false;
            StatServer.ServerWorkingHours = 0;
            StatServer.ServerWorkingMinutes = 0;
            StatServer.ServerWorkingSeconds = 0;
        }
        private int? _tickPeriod;
        public int? TickPeriod 
        { 
            get { return _tickPeriod ?? (_tickPeriod = 5000); }
            set { _tickPeriod = value; }
        }

        public string ConnectionString { get; set; }
        public List<StatItem> Statistic { get; set; }
        public StatServerItem StatServer { get; set; }
        
        public int ConnectectedClientsCount { get { return StatServer.ClientCount; } }
        public int TotalOrdersCount { get { return StatServer.DealCount; } }
        public string ServerTimeWorking { get
        {
            return string.Format("{0}:{1}:{2}",
                StatServer.ServerWorkingHours,
                StatServer.ServerWorkingMinutes,
                StatServer.ServerWorkingSeconds);
        } }
        public bool ServerStatus { get { return StatServer.IsServerAvailable; } }
        public string ServerName { get { return StatServer.ServerName; } }
        public decimal TotalOrdersValue { get { return StatServer.DealValue; } }
    }

}
