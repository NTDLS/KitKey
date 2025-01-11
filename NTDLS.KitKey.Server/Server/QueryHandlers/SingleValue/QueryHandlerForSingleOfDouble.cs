using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDouble;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfDouble(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkSingleOfDoubleSetReply KkSingleOfDoubleSet(RmContext context, KkSingleOfDoubleSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfDoubleSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfDoubleSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDoubleGetReply KkSingleOfDoubleGet(RmContext context, KkSingleOfDoubleGet param)
        {
            try
            {
                return new KkSingleOfDoubleGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Double?>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfDoubleGetReply(ex.GetBaseException());
            }
        }
    }
}
