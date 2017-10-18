using System;

namespace Exchange.Common
{
    public class ServerLogItem
    {
        public DateTime Date { get; set; }
        public EventTypeEnum EventType { get; set; }
        public string Data { get; set; }
        public Guid ServerId { get; set; }
        public int AvailableClientsCount { get; set; }
    }
}
