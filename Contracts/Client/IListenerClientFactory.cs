using EventHubMonitor.Contracts.Configuration;

namespace EventHubMonitor.Contracts.Client {

    public interface IListenerClientFactory {

        public IListenerClient Construct(EventHubListenerConfig config);

        public IListenerClient Construct();
    }
}