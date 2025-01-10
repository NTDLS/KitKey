using NTDLS.KitKey.Shared.Payload.ListOf.ListOfString;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfString(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkListOfStringPushLastReply KkListOfStringPushLast(RmContext context, KkListOfStringPushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfStringPushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfStringPushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfStringPushFirstReply KkListOfStringPushFirst(RmContext context, KkListOfStringPushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfStringPushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfStringPushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfStringGetAllReply KkListOfStringGetAll(RmContext context, KkListOfStringGetAll param)
        {
            try
            {
                return new KkListOfStringGetAllReply(true)
                {
                    List = _keyStoreServer.GetList<String>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfStringGetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfStringGetLastReply KkListOfStringGetLast(RmContext context, KkListOfStringGetLast param)
        {
            try
            {
                return new KkListOfStringGetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<String>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfStringGetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfStringGetFirstReply KkListOfStringGetFirst(RmContext context, KkListOfStringGetFirst param)
        {
            try
            {
                return new KkListOfStringGetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<String>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfStringGetFirstReply(ex.GetBaseException());
            }
        }
    }
}
