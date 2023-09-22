using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;
using EventHubMonitor.Services.Client;
using Microsoft.Extensions.Logging;

namespace Services.Client {

    public class SenderClientFactory : ISenderClientFactory {
        private readonly ILogger<ISenderClient> _logger;

        public SenderClientFactory(ILogger<ISenderClient> logger) {
            _logger = logger;
        }

        public ISenderClient Construct(EventHubSenderConfig config) {
            return new SenderClient(config, _logger);
        }

        public ISenderClient Construct(EventHubSenderConfig config, EventToSend evt) {
            var client = new SenderClient(config, _logger);
            client.EventToSend = evt;
            return client;
        }

        public ISenderClient Construct() {
            return new SenderClient(new EventHubSenderConfig(), _logger);
        }
    }
}