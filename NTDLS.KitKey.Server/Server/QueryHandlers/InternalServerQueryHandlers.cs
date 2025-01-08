using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _mqServer = mqServer;

        public KkCreateStoreReply KkCreateStoreQuery(RmContext context, KkCreateStore param)
        {
            try
            {
                _mqServer.CreateStore(param.StoreConfiguration);
                return new KkCreateStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkCreateStoreReply(ex.GetBaseException());
            }
        }

        public KkDeleteStoreReply KkDeleteStoreQuery(RmContext context, KkDeleteStore param)
        {
            try
            {
                _mqServer.DeleteStore(param.StoreName);
                return new KkDeleteStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteStoreReply(ex.GetBaseException());
            }
        }

        public KkPurgeStoreReply KkPurgeStoreQuery(RmContext context, KkPurgeStore param)
        {
            try
            {
                _mqServer.PurgeStore(param.StoreName);
                return new KkPurgeStoreReply(true);
            }
            catch (Exception ex)
            {
                return new KkPurgeStoreReply(ex.GetBaseException());
            }
        }

        public KkUpsertReply KkUpsertQuery(RmContext context, KkUpsert param)
        {
            try
            {
                _mqServer.Upsert(param.StoreName, param.Key, param.Value);
                return new KkUpsertReply(true);
            }
            catch (Exception ex)
            {
                return new KkUpsertReply(ex.GetBaseException());
            }
        }
    }
}
