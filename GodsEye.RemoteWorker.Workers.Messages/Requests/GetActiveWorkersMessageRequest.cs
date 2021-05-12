namespace GodsEye.RemoteWorker.Workers.Messages.Requests
{
    public class GetActiveWorkersMessageRequest : IRequestResponseMessage
    {
        public string MessageId { get; set; }
    }
}
