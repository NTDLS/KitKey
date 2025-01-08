using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkPurgeStore(string queueName)
        : IRmQuery<CMqPurgeQueueQueryReply>
    {
        public string QueueName { get; set; } = queueName;
    }

    public class CMqPurgeQueueQueryReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public CMqPurgeQueueQueryReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public CMqPurgeQueueQueryReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public CMqPurgeQueueQueryReply()
        {
        }
    }
}
