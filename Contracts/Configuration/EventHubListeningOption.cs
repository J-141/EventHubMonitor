namespace EventHubMonitor.Contracts.Configuration
{
    public class EventHubListeningOption
    {
        public string Partition { set; get; } = "";
        public int MaximumWaitTimeInSecond { set; get; } = 0;
    }
}