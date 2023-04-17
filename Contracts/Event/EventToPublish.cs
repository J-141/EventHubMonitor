namespace Contracts.Event {

    public class EventToPublish {
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public string CorrelationId { get; set; } = "";
        public string PartitionKey { get; set; } = "";
        public string Body { get; set; } = "";
    }
}