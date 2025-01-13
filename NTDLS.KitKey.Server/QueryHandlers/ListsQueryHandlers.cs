using NTDLS.KitKey.Shared.Payload.ListOf;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.QueryHandlers
{
    internal class ListsQueryHandlers<T>(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkListOfPushFirstValueReply KkListOfPushFirstValue(KkListOfPushFirstValue<T> param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfPushFirstValueReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfPushFirstValueReply(ex.GetBaseException());
            }
        }

        public KkListOfPushFirstItemReply KkListOfPushFirstItem(KkListOfPushFirstItem<T> param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListItem);
                return new KkListOfPushFirstItemReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfPushFirstItemReply(ex.GetBaseException());
            }
        }

        public KkListOfPushLastValueReply KkListOfPushLastValue(KkListOfPushLastValue<T> param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfPushLastValueReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfPushLastValueReply(ex.GetBaseException());
            }
        }

        public KkListOfPushLastItemReply KkListOfPushLastItem(KkListOfPushLastItem<T> param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListItem);
                return new KkListOfPushLastItemReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfPushLastItemReply(ex.GetBaseException());
            }
        }


        public KkListOfGetAllReply<T> KkListOfGetAll(KkListOfGetAll<T> param)
        {
            try
            {
                return new KkListOfGetAllReply<T>(true)
                {
                    List = _keyStoreServer.GetList<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGetAllReply<T>(ex.GetBaseException());
            }
        }

        public KkListOfGetLastReply<T> KkListOfGetLast(KkListOfGetLast<T> param)
        {
            try
            {
                return new KkListOfGetLastReply<T>(true)
                {
                    Value = _keyStoreServer.GetLast<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGetLastReply<T>(ex.GetBaseException());
            }
        }

        public KkListOfGetFirstReply<T> KkListOfGetFirst(KkListOfGetFirst<T> param)
        {
            try
            {
                return new KkListOfGetFirstReply<T>(true)
                {
                    Value = _keyStoreServer.GetFirst<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGetFirstReply<T>(ex.GetBaseException());
            }
        }
    }
}
