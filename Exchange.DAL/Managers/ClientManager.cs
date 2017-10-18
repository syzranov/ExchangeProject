using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Exchange.Common;

namespace Exchange.DAL.Managers
{
    public class ClientManager
    {
        public static void Save(ClientItem client, string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                var c = new Client
                {
                    Id = client.Id,
                    StartDateTime = client.StartDate,
                    EndDateTime = client.EndDate
                };

                ctx.Clients.InsertOnSubmit(c);
                ctx.SubmitChanges();
            }
        }
        public static void Update(ClientItem client, string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                var c = ctx.Clients.Single(x => x.Id == client.Id);
                c.EndDateTime = client.EndDate;
                ctx.SubmitChanges();
            }
        }
        public static List<ClientItem> GetActiveList(string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                return ctx.Clients.Where(x => x.EndDateTime == null)
                    .Select(y => new ClientItem()
                    {
                        Id = y.Id,
                        StartDate = y.StartDateTime,
                        EndDate = y.EndDateTime
                    }).ToList();
            }
        }
    }
}
