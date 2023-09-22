using System.Text.Json.Serialization;

namespace EventHubMonitor.Contracts.Event {

    [Serializable]
    public class EventToSend {
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public string PartitionKey { get; set; } = "";
        public string Body { get; set; } = "";
    }
}