using System;

namespace Exchange.Monitor
{
    public interface IMonitor : IDisposable
    {
        event StatisticUpdatedHandler StatisticUpdated;
        event StartNotificateHandler StartNotificate;
        event StopNotificateHandler StopNotificate;

        void StartMonitor();
        void StopMonitor();        
    }
}