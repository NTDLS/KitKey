using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;
using System.Net;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(CMqServer mqServer)
        : IRmMessageHandler
    {
        private readonly CMqServer _mqServer = mqServer;

        public CMqCreateQueueQueryReply CreateQueueQuery(RmContext context, CMqCreateStore param)
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

        public CMqDeleteQueueQueryReply DeleteQueueQuery(RmContext context, CMqDeleteStore param)
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

        public CMqPurgeQueueQueryReply PurgeQueueQuery(RmContext context, CMqPurgeStore param)
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

        public CMqUpsertReply EnqueueMessageToQueue(RmContext context, CMqUpsert param)
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
