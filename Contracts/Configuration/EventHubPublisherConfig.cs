namespace Contracts.Configuration {

    public class EventHubPublisherConfig {
        public string ListenerName { set; get; } = "New publisher";
        public string EventhubName { set; get; } = "";
        public string ConnectionString { set; get; } = "";
    }
}