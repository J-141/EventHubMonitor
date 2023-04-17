using Contracts.Configuration;
using Contracts.Event;

namespace EventHubMonitor.Contracts.Client {

    public interface IPublisherClient : IAsyncDisposable {
        public Guid Id { get; }
        public bool IsConnected { get; set; }
        public EventHubPublisherConfig Config { get; }

        public void Connect();

        public void Connect(string tokenCredential);

        public void Send(EventToPublish evt);
    }
}