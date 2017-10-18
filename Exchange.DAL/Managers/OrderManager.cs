using System;

namespace Exchange.DAL.Managers
{
    public class OrderManager
    {
        public static void Save(Common.OrderItem orderItem, string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                var o = new Order
                {
                    ClientId = orderItem.ClientId,
                    PriceId = orderItem.PriceId,
                    DealDate = orderItem.Date,
                    DealValue = orderItem.Value
                };

                ctx.Orders.InsertOnSubmit(o);
                ctx.SubmitChanges();

                orderItem.Id = o.Id;
            }
        }
    }
}
