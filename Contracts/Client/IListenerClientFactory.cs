using EventHubMonitor.Contracts.Configuration;

namespace EventHubMonitor.Contracts.Client
{
    public interface IListenerClientFactory
    {
        public IListenerClient ConstructFromConfig(EventHubListenerConfig config);
    }
}