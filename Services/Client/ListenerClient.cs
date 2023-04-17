﻿using Azure.Messaging.EventHubs.Consumer;
using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using System.Text;
using EventHubMonitor.Contracts.Event;

namespace EventHubMonitor.Services.Client {
    public class ListenerClient : IListenerClient {
        public Guid Id { get; } = new Guid();
        private EventHubConsumerClient _consumer;
        private CancellationTokenSource _cancellationTokenSource;
        public EventHubListenerConfig Config { get; }
        public List<EventToDisplay> EventsListened { get; private set; }
        public List<EventToDisplay> EventsRead { get; private set; }
        public bool IsListening { get; private set; } = false;
        public bool IsConnected { get; set; } = false;

        public ListenerClient(EventHubListenerConfig config) {
            Config = config;
            _cancellationTokenSource = new CancellationTokenSource();
            EventsListened = new List<EventToDisplay>();
            EventsRead = new List<EventToDisplay>();
        }

        public void ClearEvents() {
            EventsListened.Clear();
            EventsRead.Clear();
        }
        public void Connect() {

            _consumer = new EventHubConsumerClient(Config.ConsumerGroup, Config.ConnectionString, Config.EventhubName);
            IsConnected = true;

        }


        public async Task ReadBatchOfEvents() {


        }
        public void StopListening() {
            if (IsListening) {
                _cancellationTokenSource.Cancel();
            }
            
        }
        public async Task StartListening( ) {
            if (!IsListening) {
                IsListening = true;
                IAsyncEnumerable<PartitionEvent> events;
                if (string.IsNullOrWhiteSpace(Config.EventHubListeningOption.Partition)) {
                    events = _consumer.ReadEventsAsync(false, cancellationToken: _cancellationTokenSource.Token);
                } else {
                    events = _consumer.ReadEventsFromPartitionAsync(Config.EventHubListeningOption.Partition, EventPosition.Latest, _cancellationTokenSource.Token);
                }
                try {
                    await foreach (var pe in events.WithCancellation(_cancellationTokenSource.Token)) {
                        EventsListened.Append(new EventToDisplay() {
                            Properties = pe.Data.Properties.ToDictionary<KeyValuePair<string, object>, string, string>(i => i.Key, i => i.Value.ToString() ?? ""),
                            EnqueuedTime = pe.Data.EnqueuedTime.ToString(),
                            CorrelationId = pe.Data.CorrelationId,
                            PartitionKey = pe.Data.PartitionKey,
                            Body = Encoding.UTF8.GetString(pe.Data.EventBody)
                        });
                    }
                }
                catch (OperationCanceledException) {
                    IsListening = false;
                }
            }
        }

        public async ValueTask DisposeAsync() {
            if (_consumer != null) { await _consumer.DisposeAsync(); }
        }
    }
}