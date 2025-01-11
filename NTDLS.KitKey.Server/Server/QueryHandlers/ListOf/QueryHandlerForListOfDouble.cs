using NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfDouble(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkListOfDoublePushLastReply KkListOfDoublePushLast(RmContext context, KkListOfDoublePushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDoublePushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDoublePushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfDoublePushFirstReply KkListOfDoublePushFirst(RmContext context, KkListOfDoublePushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDoublePushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDoublePushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfDoubleGetAllReply KkListOfDoubleGetAll(RmContext context, KkListOfDoubleGetAll param)
        {
            try
            {
                return new KkListOfDoubleGetAllReply(true)
                {
                    List = _keyStoreServer.GetList<double>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDoubleGetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfDoubleGetLastReply KkListOfDoubleGetLast(RmContext context, KkListOfDoubleGetLast param)
        {
            try
            {
                return new KkListOfDoubleGetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<double>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDoubleGetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfDoubleGetFirstReply KkListOfDoubleGetFirst(RmContext context, KkListOfDoubleGetFirst param)
        {
            try
            {
                return new KkListOfDoubleGetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<double>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDoubleGetFirstReply(ex.GetBaseException());
            }
        }
    }
}
