using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfString;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfString(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkSingleOfStringSetReply KkSingleOfStringSet(RmContext context, KkSingleOfStringSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfStringSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfStringGetReply KkSingleOfStringGet(RmContext context, KkSingleOfStringGet param)
        {
            try
            {
                return new KkSingleOfStringGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<String?>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringGetReply(ex.GetBaseException());
            }
        }
    }
}
