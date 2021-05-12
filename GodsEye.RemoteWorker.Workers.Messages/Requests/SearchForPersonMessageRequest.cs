using GodsEye.Utility.Application.Helpers.Helpers.Hashing;

namespace GodsEye.RemoteWorker.Workers.Messages.Requests
{
    public class SearchForPersonMessageRequest : IRequestResponseMessage
    {
        private string _messageId;
        public string MessageId
        {
            set => _messageId = value;
            get => _messageId ??= StringContentHasherHelpers
                .GetChecksumOfStringContent(MessageContent);
        }

        public string MessageContent { get; set; }
    }
}
