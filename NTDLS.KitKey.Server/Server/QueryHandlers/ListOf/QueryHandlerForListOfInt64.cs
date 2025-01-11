using NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfInt64(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkListOfInt64PushLastReply KkListOfInt64PushLast(RmContext context, KkListOfInt64PushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt64PushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt64PushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64PushFirstReply KkListOfInt64PushFirst(RmContext context, KkListOfInt64PushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt64PushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt64PushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64GetAllReply KkListOfInt64GetAll(RmContext context, KkListOfInt64GetAll param)
        {
            try
            {
                return new KkListOfInt64GetAllReply(true)
                {
                    List = _keyStoreServer.GetList<Int64>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt64GetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64GetLastReply KkListOfInt64GetLast(RmContext context, KkListOfInt64GetLast param)
        {
            try
            {
                return new KkListOfInt64GetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<long>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt64GetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64GetFirstReply KkListOfInt64GetFirst(RmContext context, KkListOfInt64GetFirst param)
        {
            try
            {
                return new KkListOfInt64GetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<long>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt64GetFirstReply(ex.GetBaseException());
            }
        }
    }
}
