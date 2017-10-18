using System;

namespace Exchange.Common
{
    public class StatServerItem
    {
        public int DealCount { get; set; }
        public decimal DealValue { get; set; }
        public int ClientCount { get; set; }
        public string ServerName { get; set; }
        public int ServerWorkingHours { get; set; }
        public int ServerWorkingMinutes { get; set; }
        public int ServerWorkingSeconds { get; set; }
        public bool IsServerAvailable { get; set; }
    }
}