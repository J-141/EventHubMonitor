using EventHubMonitor.Contracts.Client;
using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Services.Client;

namespace Services.Client
{
    public class ListenerClientFactory : IListenerClientFactory
    {
        public IListenerClient ConstructFromConfig(EventHubListenerConfig config)
        {
            return new ListenerClient(config);
        }
    }
}