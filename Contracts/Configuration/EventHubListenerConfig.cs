namespace EventHubMonitor.Contracts.Configuration
{
    public class EventHubListenerConfig
    {
        public string ListenerName { set; get; } = "New listener";
        public string EventhubName { set; get; } = "";
        public string ConnectionString { set; get; } = "";
        public string ConsumerGroup { set; get; } = "$Default";
        public EventHubListeningOption EventHubListeningOption { set; get; } = new EventHubListeningOption();
    }
}