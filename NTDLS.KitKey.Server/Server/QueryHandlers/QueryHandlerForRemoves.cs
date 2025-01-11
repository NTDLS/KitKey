using NTDLS.KitKey.Shared.Payload.Deletes;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForRemoves(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

        public KkRemoveKeyReply KkRemoveKey(RmContext context, KkRemoveKey param)
        {
            try
            {
                _keyStoreServer.RemoveKey(param.StoreKey, param.ValueKey);
                return new KkRemoveKeyReply(true);
            }
            catch (Exception ex)
            {
                return new KkRemoveKeyReply(ex.GetBaseException());
            }
        }

        public KkRemoveListItemByKeyReply KkRemoveListItemByKey(RmContext context, KkRemoveListItemByKey param)
        {
            try
            {
                _keyStoreServer.RemoveListItemByKey(param.StoreKey, param.ListKey, param.ListItemKey);
                return new KkRemoveListItemByKeyReply(true);
            }
            catch (Exception ex)
            {
                return new KkRemoveListItemByKeyReply(ex.GetBaseException());
            }
        }
    }
}
