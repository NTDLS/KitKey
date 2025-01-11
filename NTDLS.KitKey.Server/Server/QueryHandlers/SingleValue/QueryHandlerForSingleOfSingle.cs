using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfSingle;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfSingle(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkSingleOfSingleSetReply KkSingleOfSingleSet(RmContext context, KkSingleOfSingleSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfSingleSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfSingleSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfSingleGetReply KkSingleOfSingleGet(RmContext context, KkSingleOfSingleGet param)
        {
            try
            {
                return new KkSingleOfSingleGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Single?>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfSingleGetReply(ex.GetBaseException());
            }
        }
    }
}
