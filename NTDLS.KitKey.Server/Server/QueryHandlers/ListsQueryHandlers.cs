using NTDLS.KitKey.Shared.Payload.ListOf;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class ListsQueryHandlers<T>(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkListOfPushLastReply KkListOfPushLast(RmContext context, KkListOfPushLast<T> param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfPushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfPushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimePushFirstReply KkListOfPushFirst(RmContext context, KkListOfPushFirst<T> param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDateTimePushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimePushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetAllReply<T> KkListOfGetAll(RmContext context, KkListOfGetAll<T> param)
        {
            try
            {
                return new KkListOfDateTimeGetAllReply<T>(true)
                {
                    List = _keyStoreServer.GetList<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetAllReply<T>(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetLastReply<T> KkListOfGetLast(RmContext context, KkListOfGetLast<T> param)
        {
            try
            {
                return new KkListOfDateTimeGetLastReply<T>(true)
                {
                    Value = _keyStoreServer.GetLast<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetLastReply<T>(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetFirstReply<T> KkListOfGetFirst(RmContext context, KkListOfGetFirst<T> param)
        {
            try
            {
                return new KkListOfDateTimeGetFirstReply<T>(true)
                {
                    Value = _keyStoreServer.GetFirst<T>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetFirstReply<T>(ex.GetBaseException());
            }
        }
    }
}
