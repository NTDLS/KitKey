using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfInt32;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfInt32(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkSingleOfInt32SetReply KkSingleOfInt32Set(RmContext context, KkSingleOfInt32Set param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfInt32SetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt32SetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt32GetReply KkSingleOfInt32Get(RmContext context, KkSingleOfInt32Get param)
        {
            try
            {
                return new KkSingleOfInt32GetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Int32>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt32GetReply(ex.GetBaseException());
            }
        }
    }
}
