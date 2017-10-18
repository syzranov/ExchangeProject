using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Exchange.Common;
using Exchange.DAL.Managers;
using Exchange.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exchange.UnitTests
{
    [TestClass]
    public class TestDBTools
    {
        private string _cs;

        [TestInitialize]
        public void Init()
        {
            _cs = ConfigTools.Instance.Get<string>("connection-string");
        }

        [TestMethod]
        public void CreateDB()
        {
            DAL.Managers.DalManager.Instance.DbManager.CreateDb(_cs);
        }

        [TestMethod]
        public void DropDB()
        {
            DAL.Managers.DalManager.Instance.DbManager.DeleteDb(_cs);
        }

        [TestMethod]
        public void ClientConnected()
        {
            for (var i = 0; i < 50; i++)
            {
                var clientItem = new ClientItem
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.Now
                };

                DAL.Managers.ClientManager.Save(clientItem, _cs);                
            }
        }
        [TestMethod]
        public void ClientUpdate()
        {
            var list = new List<ClientItem>();
            for (var i = 0; i < 10; i++)
            {
                var clientItem = new ClientItem
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.Now
                };

                list.Add(clientItem);

                DAL.Managers.ClientManager.Save(clientItem, _cs);
            }

            Thread.Sleep(5000);
            foreach (var item in list)
            {
                item.EndDate = DateTime.Now;
                DAL.Managers.ClientManager.Update(item, _cs);
            }
        }

        [TestMethod]
        public void PriceRetrive()
        {
            var list = DAL.Managers.PriceManager.Get(_cs);
        }

        [TestMethod]
        public void PriceUpdate()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(3000);
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

                DAL.Managers.PriceManager.Save(list, _cs);                
            }
        }

        [TestMethod]
        public void OrderCreate()
        {

            var _clist = new List<CurrencyTypeEnum>
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
            var r = new Random(1);
             
            // request client
            var clientList = DAL.Managers.ClientManager.GetActiveList(_cs);

            // request price
            var pricelist = DAL.Managers.PriceManager.Get(_cs);

            var orderList = new List<OrderItem>();
            foreach (var clientItem in clientList)
            {
                var rc = _clist[r.Next(0, _clist.Count - 1)];
                var price = pricelist.Single(x => x.CurrencyCode == rc);

                var order = new OrderItem(clientItem.Id)
                {
                    CommandType = CommandTypeEnum.RequestOrder,
                    CurrencyType = rc,
                    Value = r.Next(1, 100000),
                    Price = price.Price,
                    PriceId = price.Id,
                    Date = DateTime.Now
                };

                orderList.Add(order);
                OrderManager.Save(order, _cs);
            }

            foreach (var orderItem in orderList)
            {
                Assert.IsNotNull(orderItem.Id);
            }
        }

        [TestMethod]
        public void MonitorTest()
        {
            var _clist = new List<CurrencyTypeEnum>
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

            var list = MonitorManager.GetCurrencyStatictic(5000, _cs);
            Assert.IsTrue(list.Count == _clist.Count);
        }
    }
}