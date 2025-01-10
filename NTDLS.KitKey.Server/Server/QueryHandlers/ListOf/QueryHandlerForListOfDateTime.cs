using NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfDateTime(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkListOfDateTimePushLastReply KkListOfDateTimePushLast(RmContext context, KkListOfDateTimePushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDateTimePushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimePushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimePushFirstReply KkListOfDateTimePushFirst(RmContext context, KkListOfDateTimePushFirst param)
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

        public KkListOfDateTimeGetAllReply KkListOfDateTimeGetAll(RmContext context, KkListOfDateTimeGetAll param)
        {
            try
            {
                return new KkListOfDateTimeGetAllReply(true)
                {
                    List = _keyStoreServer.GetList<DateTime>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetLastReply KkListOfDateTimeGetLast(RmContext context, KkListOfDateTimeGetLast param)
        {
            try
            {
                return new KkListOfDateTimeGetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<DateTime>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetFirstReply KkListOfDateTimeGetFirst(RmContext context, KkListOfDateTimeGetFirst param)
        {
            try
            {
                return new KkListOfDateTimeGetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<DateTime>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetFirstReply(ex.GetBaseException());
            }
        }

    }
}
