using System;

namespace Exchange.Common
{
    public static class OrderExtentions
    {
        public static void ToOrder(this Message message, out OrderItem orderItem)
        {
            orderItem = (OrderItem)Convert.ChangeType(message.Data, typeof(OrderItem));
        }
        public static OrderItem ToOrder(this Object obj)
        {
            return (OrderItem)Convert.ChangeType(obj, typeof(OrderItem));
        }

        public static void Clear(this OrderItem orderItem)
        {
            orderItem.CommandType = CommandTypeEnum.None;
            orderItem.CurrencyType = CurrencyTypeEnum.NONE;
            orderItem.Id = null;
            orderItem.Price = 0;
            orderItem.Value = 0;
        }
        public static void Save(this OrderItem orderItem)
        {
            // TODO: Save ..
        }
    }
}