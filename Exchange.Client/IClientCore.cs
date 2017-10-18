using System;

namespace Exchange.Client
{
    public interface IClientCore : IDisposable
    {
        event ClientConnectedHandler ClientConnected;
        event ClientDisConnectedHandler ClientDisconnected;
        event ClientMessageReceivedHandler MessageReceived;
        event ClientMessageSubmittedHandler MessageSubmitted;

        void Start(string ip, int port, int timeout, string cs);
        void Stop();

        Guid Id { get; }
        DateTime StartTime { get; } 
    }
}
