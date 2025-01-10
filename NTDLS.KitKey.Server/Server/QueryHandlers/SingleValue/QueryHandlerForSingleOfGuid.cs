using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfGuid;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForSingleOfGuid(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkSingleOfGuidSetReply KkSingleOfGuidSet(RmContext context, KkSingleOfGuidSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfGuidSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfGuidSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfGuidGetReply KkSingleOfGuidGet(RmContext context, KkSingleOfGuidGet param)
        {
            try
            {
                return new KkSingleOfGuidGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Guid>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfGuidGetReply(ex.GetBaseException());
            }
        }
    }
}
