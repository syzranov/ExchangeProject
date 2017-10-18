using System;

namespace Exchange.Server
{
    public interface IServerCore : IDisposable
    {
        event ServerStartedHandler ServerStarted;
        event ServerStoppedHandler ServerStopped;

        event ClientConnectedHandler ClientConnected;
        event ClientDisconnectedHandler ClientDisconnected;

        event MessageReceivedHandler MessageReceived;
        event MessageSubmittedHandler MessageSubmitted;

        void Start(string ip, int port, string cs);
        void Stop();

        Guid Id { get; }
        DateTime StartTime { get; } 
    }
}
