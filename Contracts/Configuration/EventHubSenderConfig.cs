namespace EventHubMonitor.Contracts.Configuration {

    [Serializable]
    public class EventHubSenderConfig {
        public string SenderName { set; get; } = "New sender";
        public string EventHubName { set; get; } = "";
        public string ConnectionString { set; get; } = "";
    }
}