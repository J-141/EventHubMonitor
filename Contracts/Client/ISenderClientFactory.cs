using EventHubMonitor.Contracts.Configuration;
using EventHubMonitor.Contracts.Event;

namespace EventHubMonitor.Contracts.Client {

    public interface ISenderClientFactory {

        public ISenderClient Construct(EventHubSenderConfig config);

        public ISenderClient Construct(EventHubSenderConfig config, EventToSend evt);

        public ISenderClient Construct();
    }
}