using System;

namespace Exchange.Common
{
    public class PriceItem
    {
        public int Id { get; set; }
        public CurrencyTypeEnum CurrencyCode { get; set; }
        public decimal Price { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
