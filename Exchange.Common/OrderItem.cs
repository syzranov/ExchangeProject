using System;

namespace Exchange.Common
{
    [Serializable]
    public class OrderItem
    {
        private readonly Guid _clientId;
        public OrderItem(Guid clientId)
        {
            _clientId = clientId;
        }
        public Guid ClientId { get { return _clientId; } }
        public CommandTypeEnum CommandType { get; set; }
        public CurrencyTypeEnum CurrencyType { get; set; }
        public int? Id { get; set; }
        public decimal Price { get; set; }
        public int PriceId { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}