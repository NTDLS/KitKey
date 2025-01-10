using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDateTime;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfDateTime(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkSingleOfDateTimeSetReply KkSingleOfDateTimeSet(RmContext context, KkSingleOfDateTimeSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfDateTimeSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfDateTimeSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDateTimeGetReply KkSingleOfDateTimeGet(RmContext context, KkSingleOfDateTimeGet param)
        {
            try
            {
                return new KkSingleOfDateTimeGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<DateTime>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfDateTimeGetReply(ex.GetBaseException());
            }
        }
    }
}
