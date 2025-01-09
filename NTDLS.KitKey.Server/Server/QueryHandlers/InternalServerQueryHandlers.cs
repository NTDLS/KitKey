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

        public KkDeleteReply KkDelete(RmContext context, KkDelete param)
        {
            try
            {
                _keyStoreServer.Delete(param.StoreName, param.Key);
                return new KkDeleteReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteReply(ex.GetBaseException());
            }
        }

        #region String.

        public KkSetStringReply KkSetString(RmContext context, KkSetString param)
        {
            try
            {
                _keyStoreServer.SetString(param.StoreName, param.Key, param.Value);
                return new KkSetStringReply(true);
            }
            catch (Exception ex)
            {
                return new KkSetStringReply(ex.GetBaseException());
            }
        }

        public KkGetStringReply KkGetString(RmContext context, KkGetString param)
        {
            try
            {
                return new KkGetStringReply(true)
                {
                    Value = _keyStoreServer.GetString(param.StoreName, param.Key)
                };
            }
            catch (Exception ex)
            {
                return new KkGetStringReply(ex.GetBaseException());
            }
        }

        #endregion

        #region List.

        public KkAppendListReply KkAppendList(RmContext context, KkAppendList param)
        {
            try
            {
                _keyStoreServer.AppendList(param.StoreName, param.Key, param.Value);
                return new KkAppendListReply(true);
            }
            catch (Exception ex)
            {
                return new KkAppendListReply(ex.GetBaseException());
            }
        }

        public KkGetListReply KkGetList(RmContext context, KkGetList param)
        {
            try
            {
                return new KkGetListReply(true)
                {
                    List = _keyStoreServer.GetList(param.StoreName, param.Key)
                };
            }
            catch (Exception ex)
            {
                return new KkGetListReply(ex.GetBaseException());
            }
        }

        #endregion
    }
}
