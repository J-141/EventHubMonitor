using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;

namespace EventHubMonitor.Contracts.Client {

    public interface IListenerClient : IAsyncDisposable {
        public Guid Id { get; }

        public event Action EventCallBack;

        public List<EventToDisplay> EventsListened { get; }
        public bool IsConnected { get; set; }
        public EventHubListenerConfig Config { get; }
        public bool IsListening { get; }

        public void Connect();

        public void Connect(string tokenCredential);

        public void ClearEvents();

        public Task StartListening();

        public void StopListening();
    }
}