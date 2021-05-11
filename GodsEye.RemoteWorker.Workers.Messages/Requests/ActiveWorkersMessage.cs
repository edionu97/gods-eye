namespace GodsEye.RemoteWorker.Workers.Messages.Requests
{
    public class ActiveWorkersMessage : IRequestResponseMessage
    {
        public string MessageId { get; set; }
    }
}
