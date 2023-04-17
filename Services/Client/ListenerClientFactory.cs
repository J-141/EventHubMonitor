using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Services.Client;
using Microsoft.Extensions.Logging;

namespace Services.Client {

    public class ListenerClientFactory : IListenerClientFactory {
        private readonly ILogger<IListenerClient> _logger;

        public ListenerClientFactory(ILogger<IListenerClient> logger) {
            _logger = logger;
        }

        public IListenerClient ConstructFromConfig(EventHubListenerConfig config) {
            return new ListenerClient(config, _logger);
        }
    }
}