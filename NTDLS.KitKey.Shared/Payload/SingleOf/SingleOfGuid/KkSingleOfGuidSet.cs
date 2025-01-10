﻿using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfGuid
{
    public class KkSingleOfGuidSet(string storeKey, string valueKey, Guid value)
        : IRmQuery<KkSingleOfGuidSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public Guid Value { get; set; } = value;
    }

    public class KkSingleOfGuidSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfGuidSetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfGuidSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfGuidSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfGuidSetReply()
        {
        }
    }
}
