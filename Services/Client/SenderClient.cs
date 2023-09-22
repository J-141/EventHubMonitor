using Azure;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;
using Microsoft.Extensions.Logging;

namespace EventHubMonitor.Services.Client {

    public class SenderClient : ISenderClient {
        public Guid Id { get; } = new Guid();

        public bool IsConnected { get => _producerClient != null; }

        public EventHubSenderConfig Config { get; }
        public EventHubProducerClient? _producerClient { get; set; } = null;

        public EventToSend? EventToSend { get; set; }

        private readonly ILogger<ISenderClient> _logger;

        public SenderClient(EventHubSenderConfig config, ILogger<ISenderClient> logger) {
            _logger = logger;
            Config = config;
            EventToSend = new EventToSend();
        }

        public void Connect() {
            _producerClient = new EventHubProducerClient(Config.ConnectionString);
        }

        public void Connect(string sasTokenCredential) {
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

            _producerClient = new EventHubProducerClient(connection);
        }

        public ValueTask DisposeAsync() {
            if (_producerClient != null) {
                return _producerClient.DisposeAsync();
            }
            return ValueTask.CompletedTask;
        }

        public void Send() {
            if (this.EventToSend != null && IsConnected) {
                var evt = new EventData(this.EventToSend.Body);
                evt.CorrelationId = EventToSend.CorrelationId;
                foreach (var kv in EventToSend.Properties.ToList()) {
                    evt.Properties.Add(kv.Key, kv.Value);
                }
                _producerClient!.SendAsync(new[] { evt }, new SendEventOptions() { PartitionKey = EventToSend.PartitionKey });
                _logger.LogInformation("Event sent.");
            }
        }
    }
}