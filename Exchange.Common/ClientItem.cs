using System;

namespace Exchange.Common
{
    public class ClientItem
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
