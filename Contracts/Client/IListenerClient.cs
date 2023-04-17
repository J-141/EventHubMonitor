using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;

namespace EventHubMonitor.Contracts.Client
{
    public interface IListenerClient: IAsyncDisposable {
        public Guid Id { get; }
        public List<EventToDisplay> EventsListened { get; }
        public List<EventToDisplay> EventsRead { get; }
        public bool IsConnected { get; set; }
        public EventHubListenerConfig Config { get; }
        public bool IsListening { get; }
        public void Connect();

        public void ClearEvents();

        public Task StartListening();
        public void StopListening();
        public Task ReadBatchOfEvents();
    }
}