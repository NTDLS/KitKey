using NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfInt32(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkListOfInt32PushLastReply KkListOfInt32PushLast(RmContext context, KkListOfInt32PushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt32PushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt32PushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32PushFirstReply KkListOfInt32PushFirst(RmContext context, KkListOfInt32PushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt32PushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt32PushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32GetAllReply KkListOfInt32GetAll(RmContext context, KkListOfInt32GetAll param)
        {
            try
            {
                return new KkListOfInt32GetAllReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt32GetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32GetLastReply KkListOfInt32GetLast(RmContext context, KkListOfInt32GetLast param)
        {
            try
            {
                return new KkListOfInt32GetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<int>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt32GetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32GetFirstReply KkListOfInt32GetFirst(RmContext context, KkListOfInt32GetFirst param)
        {
            try
            {
                return new KkListOfInt32GetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<int>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt32GetFirstReply(ex.GetBaseException());
            }
        }
    }
}
