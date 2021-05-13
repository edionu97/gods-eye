namespace GodsEye.RemoteWorker.Workers.Messages.Requests
{
    public class StopSearchingForPersonMessageRequest : IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
    }
}
