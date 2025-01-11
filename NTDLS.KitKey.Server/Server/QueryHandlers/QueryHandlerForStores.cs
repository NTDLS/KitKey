using NTDLS.KitKey.Shared.Payload.Stores;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForStores(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkStoreCreateReply KkStoreCreate(RmContext context, KkStoreCreate param)
        {
            try
            {
                _keyStoreServer.CreateStore(param.StoreConfiguration);
                return new KkStoreCreateReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreCreateReply(ex.GetBaseException());
            }
        }

        public KkStoreDeleteReply KkStoreDelete(RmContext context, KkStoreDelete param)
        {
            try
            {
                _keyStoreServer.DeleteStore(param.StoreKey);
                return new KkStoreDeleteReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreDeleteReply(ex.GetBaseException());
            }
        }

        public KkStorePurgeReply KkStorePurge(RmContext context, KkStorePurge param)
        {
            try
            {
                _keyStoreServer.PurgeStore(param.StoreKey);
                return new KkStorePurgeReply(true);
            }
            catch (Exception ex)
            {
                return new KkStorePurgeReply(ex.GetBaseException());
            }
        }

        public KkStoreFlushAllCachesReply KkStoreFlushAllCaches(RmContext context, KkStoreFlushAllCaches param)
        {
            try
            {
                _keyStoreServer.FlushCache();
                return new KkStoreFlushAllCachesReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreFlushAllCachesReply(ex.GetBaseException());
            }
        }

        public KkStoreFlushCacheReply KkStoreFlushCache(RmContext context, KkStoreFlushCache param)
        {
            try
            {
                _keyStoreServer.FlushCache(param.StoreKey);
                return new KkStoreFlushCacheReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreFlushCacheReply(ex.GetBaseException());
            }
        }
    }
}
