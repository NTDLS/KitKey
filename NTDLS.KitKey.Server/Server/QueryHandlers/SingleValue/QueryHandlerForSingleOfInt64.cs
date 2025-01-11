using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfInt64;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfInt64(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;


        public KkSingleOfInt64SetReply KkSingleOfInt64Set(RmContext context, KkSingleOfInt64Set param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfInt64SetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt64SetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt64GetReply KkSingleOfInt64Get(RmContext context, KkSingleOfInt64Get param)
        {
            try
            {
                return new KkSingleOfInt64GetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Int64?>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt64GetReply(ex.GetBaseException());
            }
        }
    }
}
