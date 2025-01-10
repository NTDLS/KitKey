using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkDeleteKeyReply KkDelete(RmContext context, KkDeleteKey param)
        {
            try
            {
                _keyStoreServer.Delete(param.StoreKey, param.ValueKey);
                return new KkDeleteKeyReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteKeyReply(ex.GetBaseException());
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

        public KkSingleOfStringSetReply KkStringSet(RmContext context, KkSingleOfStringSet param)
        {
            try
            {
                _keyStoreServer.SetValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfStringSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfStringGetReply KkStringGet(RmContext context, KkSingleOfStringGet param)
        {
            try
            {
                return new KkSingleOfStringGetReply(true)
                {
                    Value = _keyStoreServer.GetValue<string>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringGetReply(ex.GetBaseException());
            }
        }

        #endregion

        #region List.

        public KkListOfStringAddReply KkListAdd(RmContext context, KkListOfStringAdd param)
        {
            try
            {
                _keyStoreServer.ListAdd(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfStringAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfStringAddReply(ex.GetBaseException());
            }
        }

        public KkListOfStringGetReply KkListGet(RmContext context, KkListOfStringGet param)
        {
            try
            {
                return new KkListOfStringGetReply(true)
                {
                    List = _keyStoreServer.ListGet(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfStringGetReply(ex.GetBaseException());
            }
        }

        #endregion
    }
}
