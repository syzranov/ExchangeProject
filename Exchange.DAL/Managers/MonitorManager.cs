using System;
using System.Collections.Generic;
using System.Linq;
using Exchange.Common;

namespace Exchange.DAL.Managers
{
    public class MonitorManager
    {
        public static StatServerItem GetServerStatictic(int ms, string cs)
        {
            var item = new StatServerItem();
            using (var ctx = new ExchangeDataContext(cs))
            {
                var orders = ctx.Orders.Where(x => 
                    x.DealDate > DateTime.Now.AddMilliseconds(-ms));

                if (orders.Any())
                {
                    item.DealCount = orders.Count();
                    item.DealValue = orders.Sum(y => y.DealValue);
                }

                var srv = ctx.ServerLogs.OrderByDescending(x=>x.EventDate)
                    .FirstOrDefault(x => x.EventTypeId == (int) EventTypeEnum.Running 
                        && x.EventDate > DateTime.Now.AddMilliseconds(-ms));

                if (srv != null)
                {
                    item.ServerName = srv.ServerId.ToString();
                    item.IsServerAvailable = true;

                    var start = ctx.ServerLogs.FirstOrDefault(x => 
                        x.EventTypeId == (int)EventTypeEnum.Started 
                            && x.ServerId == srv.ServerId);

                    if (start != null)
                    {
                        TimeSpan diff = DateTime.Now - start.EventDate;
                        item.ServerWorkingHours = diff.Hours;
                        item.ServerWorkingMinutes = diff.Minutes;
                        item.ServerWorkingSeconds = diff.Seconds;
                    }

                    item.ClientCount = srv.AvailableClientsCount;
                }
            }
            return item;
        }

        public static List<StatItem> GetCurrencyStatictic(int ms, string cs)
        {
            var list = new List<StatItem>();
            using (var ctx = new ExchangeDataContext(cs))
            {
                var orders = ctx.Orders.Where(x => x.DealDate > DateTime.Now.AddMilliseconds(-ms)).ToList();
                if (orders.Any())
                {
                    var pr = orders.GroupBy(x => x.PriceId).ToList();
                    list.AddRange(from p in pr
                        let key = p.Key
                        where key != null
                        let pp = ctx.Prices.First(x => x.Id == key)
                        select new StatItem()
                        {
                            CurrencyType = (CurrencyTypeEnum) (Convert.ToInt32(pp.CurrencyCode)), 
                            Name = ((CurrencyTypeEnum) (Convert.ToInt32(pp.CurrencyCode))).ToString(), 
                            PriceId = (int)key,
                            Price = pp.CurrencyPrice, 
                            DealCount = orders.Count(x => x.PriceId == key), 
                            MiddleValue = orders.Where(y => y.PriceId == key).Average(x => x.DealValue)
                        });
                }
            }

            return list;
        }
    }
}
