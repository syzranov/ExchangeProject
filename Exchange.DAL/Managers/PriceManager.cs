using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Exchange.Common;
using Exchange.Utils;

namespace Exchange.DAL.Managers
{
    public class PriceManager
    {
        public static void Save(List<PriceItem> prices, string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                var list = prices.Select(priceItem => new Price
                {
                    DateUpd = priceItem.DateUpdate, 
                    CurrencyCode = ((int)priceItem.CurrencyCode).ToString(CultureInfo.InvariantCulture), 
                    CurrencyPrice = priceItem.Price,
                }).ToList();

                ctx.Prices.InsertAllOnSubmit(list);
                ctx.SubmitChanges();
            }
        }
        public static List<PriceItem> Get(string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                return 
                 (from h in ctx.Prices 
                 orderby h.DateUpd descending 
                 group h by h.CurrencyCode into g 
                 let f = g.FirstOrDefault() 
                 where f != null
                 select new PriceItem
                 {
                     Id = f.Id, 
                     CurrencyCode = (CurrencyTypeEnum)Convert.ToInt32(f.CurrencyCode),
                     Price = f.CurrencyPrice,
                     DateUpdate = f.DateUpd
                 }).ToList<PriceItem>();
            }
        }

        public static void AutoUpdatePrices(string cs)
        {
            var thread = new Thread(() => UpdatePriceList(cs));
            thread.Start();
        }

        public static void Update(string cs)
        {
            var list = new List<PriceItem>();
            var dt = DateTime.Now;

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.USD,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.RUB,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.JPY,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.GBP,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.EUR,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.CNY,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.CHF,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.CAD,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            list.Add(new PriceItem
            {
                CurrencyCode = CurrencyTypeEnum.BRL,
                DateUpdate = dt,
                Price = RandData.Instance.GetRandCurrencyPrice()
            });

            Save(list, cs);            
        }
        private static void UpdatePriceList(string cs)
        {
            while (true)
            {
                Thread.Sleep(30000);
                Update(cs);
            }
        }
    }
}
