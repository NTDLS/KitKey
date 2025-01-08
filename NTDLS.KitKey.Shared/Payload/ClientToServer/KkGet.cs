using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkGet(string storeName, string key)
        : IRmQuery<CMqGetReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
    }

    public class CMqGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Value { get; set; }

        public CMqGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public CMqGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public CMqGetReply()
        {
        }
    }
}
