namespace EventHubMonitor.Contracts.Event
{
    public class EventToDisplay
    {
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public string EnqueuedTime { get; set; } = "";
        public string CorrelationId { get; set; } = "";
        public string PartitionKey { get; set; } = "";
        public string Body { get; set; } = "";
    };
}