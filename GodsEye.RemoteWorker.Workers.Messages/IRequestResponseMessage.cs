namespace GodsEye.RemoteWorker.Workers.Messages
{
    public interface IRequestResponseMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
    }
}
