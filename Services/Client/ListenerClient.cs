using Azure;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EventHubMonitor.Services.Client {

    public class ListenerClient : IListenerClient {
        private readonly ILogger<IListenerClient> _logger;

        public Guid Id { get; } = new Guid();
        private EventHubConsumerClient? _consumerClient { get; set; } = null;
        private CancellationTokenSource _cancellationTokenSource;
        public EventHubListenerConfig Config { get; }
        public List<EventToDisplay> EventsListened { get; private set; }
        public bool IsListening { get; private set; } = false;
        public bool IsConnected { get => _consumerClient != null; }

        public event Action _callbackEvent;

        public event Action EventCallBack {
            add {
                _callbackEvent += value;
            }
            remove {
                _callbackEvent -= value;
            }
        }

        public ListenerClient(EventHubListenerConfig config, ILogger<IListenerClient> logger) {
            Config = config;
            _cancellationTokenSource = new CancellationTokenSource();
            EventsListened = new List<EventToDisplay>();
            _logger = logger;
        }

        public void ClearEvents() {
            EventsListened.Clear();
        }

        public async Task ConnectAsync() {
            StopListening();
            if (_consumerClient != null) {
                await _consumerClient.DisposeAsync();
            }
            _consumerClient = new EventHubConsumerClient(Config.ConsumerGroup, Config.ConnectionString, Config.EventHubName);
        }

        public async Task ConnectAsync(string sasTokenCredential) {
            StopListening();
            if (_consumerClient != null) {
                await _consumerClient.DisposeAsync();
            }

            var properties = Config.ConnectionString.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Split('=', 2, StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(a => a[0], a => a[1]);
            if (!properties.TryGetValue("Endpoint", out var endpoint) ||
                !properties.TryGetValue("SharedAccessKeyName", out var sharedAccessKeyName) ||
                !properties.TryGetValue("SharedAccessKey", out var sharedAccessKey)) {
                throw new InvalidOperationException("Invalid connection string");
            }
            string resourceUri = new UriBuilder(endpoint).Host;
            var connection = new EventHubConnection(resourceUri, Config.EventHubName, new AzureSasCredential(sasTokenCredential), new EventHubConnectionOptions {
                TransportType = EventHubsTransportType.AmqpWebSockets
            });

            _consumerClient = new EventHubConsumerClient(Config.ConsumerGroup, connection);
        }

        public void StopListening() {
            if (IsListening) {
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task StartListening() {
            if (!IsConnected) {
                throw new InvalidOperationException("Not connected.");
            }
            if (!IsListening) {
                IsListening = true;

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();

                IAsyncEnumerable<PartitionEvent> events;
                if (string.IsNullOrWhiteSpace(Config.EventHubListeningOption.Partition)) {
                    events = _consumerClient!.ReadEventsAsync(Config.EventHubListeningOption.ReadFromBeginning, cancellationToken: _cancellationTokenSource.Token);
                }
                else {
                    events = _consumerClient!.ReadEventsFromPartitionAsync(Config.EventHubListeningOption.Partition, Config.EventHubListeningOption.ReadFromBeginning ? EventPosition.Earliest : EventPosition.Latest, _cancellationTokenSource.Token);
                }
                _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(Config.EventHubListeningOption.MaxWaitingMins));
                int count = 0;
                try {
                    await foreach (var pe in events.WithCancellation(_cancellationTokenSource.Token)) {
                        EventsListened.Add(new EventToDisplay() {
                            Properties = pe.Data.Properties.ToDictionary<KeyValuePair<string, object>, string, string>(i => i.Key, i => i.Value.ToString() ?? ""),
                            EnqueuedTime = pe.Data.EnqueuedTime.ToString(),
                            CorrelationId = pe.Data.CorrelationId,
                            PartitionKey = pe.Data.PartitionKey,
                            Body = Encoding.UTF8.GetString(pe.Data.EventBody)
                        });
                        count++;
                        if (count >= Config.EventHubListeningOption.BatchSize)
                            throw new OperationCanceledException();
                        _callbackEvent?.Invoke();
                    }
                }
                catch (OperationCanceledException) {
                    IsListening = false;
                }
            }
        }

        public async ValueTask DisposeAsync() {
            StopListening();
            if (_consumerClient != null) { _cancellationTokenSource.Dispose(); await _consumerClient.DisposeAsync(); }
        }
    }
}