using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkCreateStoreReply KkCreateStore(RmContext context, KkCreateStore param)
        {
            try
            {
                _keyStoreServer.CreateStore(param.StoreConfiguration);
                return new KkCreateStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkCreateStoreReply(ex.GetBaseException());
            }
        }

        public KkDeleteStoreReply KkDeleteStore(RmContext context, KkDeleteStore param)
        {
            try
            {
                _keyStoreServer.DeleteStore(param.StoreName);
                return new KkDeleteStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteStoreReply(ex.GetBaseException());
            }
        }

        public KkPurgeStoreReply KkPurgeStore(RmContext context, KkPurgeStore param)
        {
            try
            {
                _keyStoreServer.PurgeStore(param.StoreName);
                return new KkPurgeStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkPurgeStoreReply(ex.GetBaseException());
            }
        }

        public KkSetReply KkSet(RmContext context, KkSet param)
        {
            try
            {
                _keyStoreServer.Set(param.StoreName, param.Key, param.Value);
                return new KkSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSetReply(ex.GetBaseException());
            }
        }

        public KkGetReply KkGet(RmContext context, KkGet param)
        {
            try
            {
                return new KkGetReply(true)
                {
                    Value = _keyStoreServer.Get(param.StoreName, param.Key)
                };
            }
            catch (Exception ex)
            {
                return new KkGetReply(ex.GetBaseException());
            }
        }

        public KkDeleteReply KkDelete(RmContext context, KkDelete param)
        {
            try
            {
                _keyStoreServer.Get(param.StoreName, param.Key);
                return new KkDeleteReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteReply(ex.GetBaseException());
            }
        }
    }
}
