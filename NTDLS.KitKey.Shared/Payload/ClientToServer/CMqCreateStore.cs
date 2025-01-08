using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class CMqCreateStore(CMqStoreConfiguration queueConfiguration)
        : IRmQuery<CMqCreateQueueQueryReply>
    {
        public CMqStoreConfiguration QueueConfiguration { get; set; } = queueConfiguration;
    }

    public class CMqCreateQueueQueryReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public CMqCreateQueueQueryReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public CMqCreateQueueQueryReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public CMqCreateQueueQueryReply()
        {
        }
    }
}
