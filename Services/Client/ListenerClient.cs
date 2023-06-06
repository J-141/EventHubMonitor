using Azure.Messaging.EventHubs.Consumer;
using Azure.Core;

using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using System.Text;
using EventHubMonitor.Contracts.Event;
using Azure;
using Azure.Messaging.EventHubs;

namespace EventHubMonitor.Services.Client {
    public class ListenerClient : IListenerClient {
        public Guid Id { get; } = new Guid();
        private EventHubConsumerClient _consumer;
        private CancellationTokenSource _cancellationTokenSource;
        public EventHubListenerConfig Config { get; }
        public List<EventToDisplay> EventsListened { get; private set; }
        public bool IsListening { get; private set; } = false;
        public bool IsConnected { get; set; } = false;

        public ListenerClient(EventHubListenerConfig config) {
            Config = config;
            _cancellationTokenSource = new CancellationTokenSource();
            EventsListened = new List<EventToDisplay>();
        }

        
        public void ClearEvents() {
            EventsListened.Clear();
        }
        public void Connect() {
            _consumer = new EventHubConsumerClient(Config.ConsumerGroup, Config.ConnectionString, Config.EventhubName);
            IsConnected = true;

        }
        public void Connect(string sasTokenCredential) {

            var properties = Config.ConnectionString.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Split('=', StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(a => a[0], a => a[1]);
            if (!properties.TryGetValue("Endpoint", out var endpoint) ||
                !properties.TryGetValue("SharedAccessKeyName", out var sharedAccessKeyName) ||
                !properties.TryGetValue("SharedAccessKey", out var sharedAccessKey)) {
                    throw new InvalidOperationException("Invalid connection string");
            }
            string resourceUri = new UriBuilder(endpoint).Host;

            var connection = new EventHubConnection(resourceUri, Config.EventhubName, new AzureSasCredential(sasTokenCredential),new EventHubConnectionOptions {
                TransportType = EventHubsTransportType.AmqpWebSockets
            });

            _consumer = new EventHubConsumerClient(Config.ConsumerGroup, connection);
            IsConnected = true;
        }


        public void StopListening() {
            if (IsListening) {
                _cancellationTokenSource.Cancel();
            }
        }
        public async Task StartListening() {
            if (!IsListening) {
                IsListening = true;

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();

                IAsyncEnumerable<PartitionEvent> events;
                if (string.IsNullOrWhiteSpace(Config.EventHubListeningOption.Partition)) {
                    events = _consumer.ReadEventsAsync(Config.EventHubListeningOption.ReadFromBeginning, cancellationToken: _cancellationTokenSource.Token);
                } else {
                    events = _consumer.ReadEventsFromPartitionAsync(Config.EventHubListeningOption.Partition, Config.EventHubListeningOption.ReadFromBeginning?EventPosition.Earliest:EventPosition.Latest, _cancellationTokenSource.Token);
                }
                _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(Config.EventHubListeningOption.MaxWaitingMins));
                int count = 0;
                try {
                    await foreach (var pe in events.WithCancellation(_cancellationTokenSource.Token)) {
                        EventsListened.Append(new EventToDisplay() {
                            Properties = pe.Data.Properties.ToDictionary<KeyValuePair<string, object>, string, string>(i => i.Key, i => i.Value.ToString() ?? ""),
                            EnqueuedTime = pe.Data.EnqueuedTime.ToString(),
                            CorrelationId = pe.Data.CorrelationId,
                            PartitionKey = pe.Data.PartitionKey,
                            Body = Encoding.UTF8.GetString(pe.Data.EventBody)
                        });
                        count++;
                        if (count >= Config.EventHubListeningOption.BatchSize) throw new OperationCanceledException();
                    }
                }
                catch (OperationCanceledException) {
                    IsListening = false;
                }
            }
        }
        
        public async ValueTask DisposeAsync() {
            if (_consumer != null) { _cancellationTokenSource.Dispose(); await _consumer.DisposeAsync();  }
        }
    }
}