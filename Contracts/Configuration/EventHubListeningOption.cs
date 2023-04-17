namespace EventHubMonitor.Contracts.Configuration
{
    public class EventHubListeningOption
    {
        public string Partition { set; get; } = "";
        public int BatchSize { set; get; } = 10;
    }
}