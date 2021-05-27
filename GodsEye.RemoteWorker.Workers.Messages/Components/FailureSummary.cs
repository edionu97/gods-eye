namespace GodsEye.RemoteWorker.Workers.Messages.Components
{
    public class FailureSummary
    {
        public string Status { get; set; }
        public string ExceptionType { get; set; }
        public string FailureDetails { get; set; }
    }
}
