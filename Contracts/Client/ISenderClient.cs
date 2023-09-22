using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;

namespace EventHubMonitor.Contracts.Client {

    public interface ISenderClient : IAsyncDisposable {
        public Guid Id { get; }
        public bool IsConnected { get; }
        public EventHubSenderConfig Config { get; }
        public EventToSend EventToSend { get; set; }

        public void Connect();

        public void Connect(string tokenCredential);

        public void Send();
    }
}