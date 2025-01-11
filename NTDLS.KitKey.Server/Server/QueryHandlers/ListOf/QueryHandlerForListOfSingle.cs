using NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfSingle(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkListOfSinglePushLastReply KkListOfSinglePushLast(RmContext context, KkListOfSinglePushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfSinglePushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfSinglePushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfSinglePushFirstReply KkListOfSinglePushFirst(RmContext context, KkListOfSinglePushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfSinglePushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfSinglePushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfSingleGetAllReply KkListOfSingleGetAll(RmContext context, KkListOfSingleGetAll param)
        {
            try
            {
                return new KkListOfSingleGetAllReply(true)
                {
                    List = _keyStoreServer.GetList<Single>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfSingleGetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfSingleGetLastReply KkListOfSingleGetLast(RmContext context, KkListOfSingleGetLast param)
        {
            try
            {
                return new KkListOfSingleGetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<Single>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfSingleGetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfSingleGetFirstReply KkListOfSingleGetFirst(RmContext context, KkListOfSingleGetFirst param)
        {
            try
            {
                return new KkListOfSingleGetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<Single>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfSingleGetFirstReply(ex.GetBaseException());
            }
        }
    }
}
