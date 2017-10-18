using System;
using System.Collections.Generic;
using Exchange.Common;
using Exchange.Extentions;

namespace Exchange.Utils
{
    public class RandData
    {
        public static readonly RandData Instance = new RandData();

        private readonly List<CurrencyTypeEnum> _clist;
        public RandData()
        {
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
        }
        public CommandTypeEnum GetRandOrderSolution()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(0, 3000) % 3 == 0
                ? CommandTypeEnum.RequestOrder
                : CommandTypeEnum.RequestOrderCancel;
        }
        public decimal GetRandCurrencyPrice()
        {
            var d = (decimal)2.0;
            return d.Next();
        }

        public decimal GetRandOrderValue()
        {
            var r = new Random(1);
            return r.Next(1, 100000);
        }

        public CurrencyTypeEnum GetRandCurrency()
        {
            var r = new Random(1);
            return _clist[r.Next(0, _clist.Count - 1)];
        }
    }
}
