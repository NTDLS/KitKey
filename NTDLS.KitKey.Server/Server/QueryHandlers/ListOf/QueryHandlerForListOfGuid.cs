using NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers.ListOf
{
    internal class QueryHandlerForListOfGuid(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkListOfGuidPushLastReply KkListOfGuidPushLast(RmContext context, KkListOfGuidPushLast param)
        {
            try
            {
                _keyStoreServer.PushLast(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfGuidPushLastReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfGuidPushLastReply(ex.GetBaseException());
            }
        }

        public KkListOfGuidPushFirstReply KkListOfGuidPushFirst(RmContext context, KkListOfGuidPushFirst param)
        {
            try
            {
                _keyStoreServer.PushFirst(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfGuidPushFirstReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfGuidPushFirstReply(ex.GetBaseException());
            }
        }

        public KkListOfGuidGetAllReply KkListOfGuidGetAll(RmContext context, KkListOfGuidGetAll param)
        {
            try
            {
                return new KkListOfGuidGetAllReply(true)
                {
                    List = _keyStoreServer.GetList<Guid>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGuidGetAllReply(ex.GetBaseException());
            }
        }

        public KkListOfGuidGetLastReply KkListOfGuidGetLast(RmContext context, KkListOfGuidGetLast param)
        {
            try
            {
                return new KkListOfGuidGetLastReply(true)
                {
                    Value = _keyStoreServer.GetLast<Guid>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGuidGetLastReply(ex.GetBaseException());
            }
        }

        public KkListOfGuidGetFirstReply KkListOfGuidGetFirst(RmContext context, KkListOfGuidGetFirst param)
        {
            try
            {
                return new KkListOfGuidGetFirstReply(true)
                {
                    Value = _keyStoreServer.GetFirst<Guid>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGuidGetFirstReply(ex.GetBaseException());
            }
        }
    }
}
