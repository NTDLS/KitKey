using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkDeleteReply KkDelete(RmContext context, KkDelete param)
        {
            try
            {
                _keyStoreServer.Delete(param.StoreKey, param.ValueKey);
                return new KkDeleteReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteReply(ex.GetBaseException());
            }
        }

        #region Key-Store.

        public KkStoreCreateReply KkStoreCreate(RmContext context, KkStoreCreate param)
        {
            try
            {
                _keyStoreServer.StoreCreate(param.StoreConfiguration);
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
                _keyStoreServer.StoreDelete(param.StoreKey);
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
                _keyStoreServer.StorePurge(param.StoreKey);
                return new KkStorePurgeReply(true);
            }
            catch (Exception ex)
            {
                return new KkStorePurgeReply(ex.GetBaseException());
            }
        }

        #endregion

        #region String.

        public KkStringSetReply KkStringSet(RmContext context, KkStringSet param)
        {
            try
            {
                _keyStoreServer.StringSet(param.StoreKey, param.ValueKey, param.Value);
                return new KkStringSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkStringSetReply(ex.GetBaseException());
            }
        }

        public KkStringGetReply KkStringGet(RmContext context, KkStringGet param)
        {
            try
            {
                return new KkStringGetReply(true)
                {
                    Value = _keyStoreServer.StringGet(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkStringGetReply(ex.GetBaseException());
            }
        }

        #endregion

        #region List.

        public KkListAddReply KkListAdd(RmContext context, KkListAdd param)
        {
            try
            {
                _keyStoreServer.ListAdd(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListAddReply(ex.GetBaseException());
            }
        }

        public KkListGetReply KkListGet(RmContext context, KkListGet param)
        {
            try
            {
                return new KkListGetReply(true)
                {
                    List = _keyStoreServer.ListGet(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListGetReply(ex.GetBaseException());
            }
        }

        #endregion
    }
}
