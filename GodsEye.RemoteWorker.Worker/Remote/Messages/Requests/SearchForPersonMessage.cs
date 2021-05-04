using GodsEye.Utility.Application.Helpers.Helpers.Hashing;

namespace GodsEye.RemoteWorker.Worker.Remote.Messages.Requests
{
    public class SearchForPersonMessage : IMessage
    {
        private string _messageId;
        public string MessageId
        {
            set => _messageId = value;
            get => _messageId ??= StringContentHasherHelpers.GetChecksumOfStringContent(MessageContent);
        }

        public string MessageContent { get; set; }
    }
}
