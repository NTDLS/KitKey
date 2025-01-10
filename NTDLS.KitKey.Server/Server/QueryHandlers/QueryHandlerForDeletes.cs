using NTDLS.KitKey.Shared.Payload.Deletes;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class QueryHandlerForDeletes(KkClient mqServer)
        : IRmMessageHandler
    {
        private readonly KkClient _keyStoreServer = mqServer;

        public KkDeleteKeyReply KkDeleteKey(RmContext context, KkDeleteKey param)
        {
            try
            {
                _keyStoreServer.DeleteKey(param.StoreKey, param.ValueKey);
                return new KkDeleteKeyReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteKeyReply(ex.GetBaseException());
            }
        }

        public KkDeleteListItemByKeyReply KkDeleteListItemByKeyReply(RmContext context, KkDeleteListItemByKey param)
        {
            try
            {
                _keyStoreServer.DeleteListItemByKey(param.StoreKey, param.ListKey, param.ListItemKey);
                return new KkDeleteListItemByKeyReply(true);
            }
            catch (Exception ex)
            {
                return new KkDeleteListItemByKeyReply(ex.GetBaseException());
            }
        }
    }
}
