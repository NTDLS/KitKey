﻿using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStoreDelete(string storeKey)
        : IRmQuery<KkStoreDeleteReply>
    {
        public string StoreKey { get; set; } = storeKey;
    }

    public class KkStoreDeleteReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStoreDeleteReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStoreDeleteReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStoreDeleteReply()
        {
        }
    }
}
