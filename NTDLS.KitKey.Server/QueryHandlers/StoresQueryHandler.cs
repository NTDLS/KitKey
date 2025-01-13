using NTDLS.KitKey.Shared.Payload.Stores;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.QueryHandlers
{
    internal class StoresQueryHandler(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkStoreCreateReply KkStoreCreate(KkStoreCreate param)
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

        public KkStoreDeleteReply KkStoreDelete(KkStoreDelete param)
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

        public KkStorePurgeReply KkStorePurge(KkStorePurge param)
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

        public KkStoreFlushAllCachesReply KkStoreFlushAllCaches(KkStoreFlushAllCaches param)
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

        public KkStoreFlushCacheReply KkStoreFlushCache(KkStoreFlushCache param)
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
