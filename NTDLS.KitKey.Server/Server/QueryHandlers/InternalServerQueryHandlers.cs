using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _mqServer = mqServer;

        public CMqCreateQueueQueryReply CreateQueueQuery(RmContext context, KkCreateStore param)
        {
            try
            {
                _mqServer.CreateStore(param.QueueConfiguration);
                return new CMqCreateQueueQueryReply(true);
            }
            catch (Exception ex)
            {
                return new CMqCreateQueueQueryReply(ex.GetBaseException());
            }
        }

        public CMqDeleteQueueQueryReply DeleteQueueQuery(RmContext context, KkDeleteStore param)
        {
            try
            {
                _mqServer.DeleteStore(param.QueueName);
                return new CMqDeleteQueueQueryReply(true);
            }
            catch (Exception ex)
            {
                return new CMqDeleteQueueQueryReply(ex.GetBaseException());
            }
        }

        public CMqPurgeQueueQueryReply PurgeQueueQuery(RmContext context, KkPurgeStore param)
        {
            try
            {
                _mqServer.PurgeStore(param.QueueName);
                return new CMqPurgeQueueQueryReply(true);
            }
            catch (Exception ex)
            {
                return new CMqPurgeQueueQueryReply(ex.GetBaseException());
            }
        }

        public CMqUpsertReply EnqueueMessageToQueue(RmContext context, KkUpsert param)
        {
            try
            {
                _mqServer.Upsert(param.StoreName, param.Key, param.Value);
                return new CMqUpsertReply(true);
            }
            catch (Exception ex)
            {
                return new CMqUpsertReply(ex.GetBaseException());
            }
        }
    }
}
