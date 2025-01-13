using NTDLS.KitKey.Shared.Payload.SingleOf;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.QueryHandlers
{
    internal class SingleValueQueryHandlers<T>(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkSetSingleOfReply KkSetSingleOf(KkSetSingleOf<T> param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSetSingleOfReply(true);
            }
            catch (Exception ex)
            {
                return new KkSetSingleOfReply(ex.GetBaseException());
            }
        }

        public KkGetSingleOfReply<T> KkGetSingleOf(KkGetSingleOf<T> param)
        {
            try
            {
                return new KkGetSingleOfReply<T>(true)
                {
                    Value = _keyStoreServer.GetSingleValue<T?>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkGetSingleOfReply<T>(ex.GetBaseException());
            }
        }
    }
}
