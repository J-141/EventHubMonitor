namespace EventHubMonitor.Contracts.Configuration {

    [Serializable]
    public class EventHubListeningOption {
        public string Partition { set; get; } = "";
        public int BatchSize { set; get; } = 10;
        public int MaxWaitingMins { set; get; } = 60;
        public bool ReadFromBeginning { set; get; } = true;
    }
}