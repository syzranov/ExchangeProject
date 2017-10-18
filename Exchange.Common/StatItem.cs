namespace Exchange.Common
{
    public class StatItem
    {
        public int PriceId { get; set; }
        public CurrencyTypeEnum CurrencyType { get; set; }
        public decimal Price { get; set; }
        public decimal MiddleValue { get; set; }
        public int DealCount { get; set; }
        public string Name { get; set; }
    }
}
