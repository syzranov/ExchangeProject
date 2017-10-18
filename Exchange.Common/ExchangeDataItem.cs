using System;

namespace Exchange.Common
{
    public class ExchangeDataItem
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Value { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
