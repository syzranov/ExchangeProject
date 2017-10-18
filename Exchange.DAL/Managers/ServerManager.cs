using System.Linq;
using Exchange.Common;

namespace Exchange.DAL.Managers
{
    public class ServerManager
    {
        public static void Add(ServerLogItem item, string cs)
        {
            using (var ctx = new ExchangeDataContext(cs))
            {
                var cl = new ServerLog()
                {
                    EventTypeId = (int)item.EventType,
                    EventDate = item.Date,
                    Data = item.Data,
                    ServerId = item.ServerId,
                    AvailableClientsCount = item.AvailableClientsCount
                };

                ctx.ServerLogs.InsertOnSubmit(cl);
                ctx.SubmitChanges();
            }
        }
    }
}
