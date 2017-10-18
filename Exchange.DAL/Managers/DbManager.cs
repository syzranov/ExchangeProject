using System.Transactions;
namespace Exchange.DAL.Managers
{
    internal class DbManager : IDbManager
    {
        public void CreateDb(string connectionString)
        {
            using (var dc = new ExchangeDataContext(connectionString))
            {
                dc.CreateDatabase();

                var currences = new[]
                {
                    new Currency {Name = "RUB", Code = "643", FullName = "Russian Rouble" },
                    new Currency {Name = "USD", Code = "840", FullName = "US Dollar"},
                    new Currency {Name = "EUR", Code = "978", FullName = "Euro"},
                    new Currency {Name = "CHF", Code = "756", FullName = "Swiss franc"},
                    new Currency {Name = "JPY", Code = "392", FullName = "Japanese Yen"},
                    new Currency {Name = "GBP", Code = "826", FullName = "British Pound"},
                    new Currency {Name = "CAD", Code = "124", FullName = "Canadian Dollar"},
                    new Currency {Name = "CNY", Code = "156", FullName = "Chinese yen"},
                    new Currency {Name = "BRL", Code = "986", FullName = "Brazilian Real"}
                };

                // заполнение справочника валют
                using (var scope = new TransactionScope())
                {
                    dc.Currencies.InsertAllOnSubmit(currences);
                    dc.SubmitChanges();
                    scope.Complete();
                }

                // заполнение справочника событий
                var eventTypes = new[]
                {
                    new EventType { Id = 1, Name = "Server started"},
                    new EventType { Id = 2, Name = "Server stopped"},
                    new EventType { Id = 3, Name = "Server running"},
                };
                
                using (var scope = new TransactionScope())
                {
                    dc.EventTypes.InsertAllOnSubmit(eventTypes);
                    dc.SubmitChanges();
                    scope.Complete();
                }
            }
        }

        public void DeleteDb(string connectionString)
        {
            using (var dc = new ExchangeDataContext(connectionString))
            {
                dc.DeleteDatabase();
            }
        }
    }
}
