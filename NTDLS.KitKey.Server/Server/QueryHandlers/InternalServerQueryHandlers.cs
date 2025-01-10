using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf;
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Server.Server.QueryHandlers
{
    internal class InternalServerQueryHandlers(KkServer mqServer)
        : IRmMessageHandler
    {
        private readonly KkServer _keyStoreServer = mqServer;

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

        #region Key-Store.

        public KkStoreCreateReply KkStoreCreate(RmContext context, KkStoreCreate param)
        {
            try
            {
                _keyStoreServer.StoreCreate(param.StoreConfiguration);
                return new KkStoreCreateReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreCreateReply(ex.GetBaseException());
            }
        }

        public KkStoreDeleteReply KkStoreDelete(RmContext context, KkStoreDelete param)
        {
            try
            {
                _keyStoreServer.StoreDelete(param.StoreKey);
                return new KkStoreDeleteReply(true);
            }
            catch (Exception ex)
            {
                return new KkStoreDeleteReply(ex.GetBaseException());
            }
        }

        public KkStorePurgeReply KkStorePurge(RmContext context, KkStorePurge param)
        {
            try
            {
                _keyStoreServer.StorePurge(param.StoreKey);
                return new KkStorePurgeReply(true);
            }
            catch (Exception ex)
            {
                return new KkStorePurgeReply(ex.GetBaseException());
            }
        }

        #endregion

        #region Single Value.

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

        public KkSingleOfStringSetReply KkSingleOfStringSet(RmContext context, KkSingleOfStringSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfStringSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfStringGetReply KkSingleOfStringGet(RmContext context, KkSingleOfStringGet param)
        {
            try
            {
                return new KkSingleOfStringGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<string>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfStringGetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt32SetReply KkSingleOfInt32Set(RmContext context, KkSingleOfInt32Set param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfInt32SetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt32SetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt32GetReply KkSingleOfInt32Get(RmContext context, KkSingleOfInt32Get param)
        {
            try
            {
                return new KkSingleOfInt32GetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Int32>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt32GetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt64SetReply KkSingleOfInt64Set(RmContext context, KkSingleOfInt64Set param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfInt64SetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt64SetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfInt64GetReply KkSingleOfInt64Get(RmContext context, KkSingleOfInt64Get param)
        {
            try
            {
                return new KkSingleOfInt64GetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Int64>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfInt64GetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfSingleSetReply KkSingleOfSingleSet(RmContext context, KkSingleOfSingleSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfSingleSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfSingleSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfSingleGetReply KkSingleOfSingleGet(RmContext context, KkSingleOfSingleGet param)
        {
            try
            {
                return new KkSingleOfSingleGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Single>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfSingleGetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDoubleSetReply KkSingleOfDoubleSet(RmContext context, KkSingleOfDoubleSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfDoubleSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfDoubleSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDoubleGetReply KkSingleOfDoubleGet(RmContext context, KkSingleOfDoubleGet param)
        {
            try
            {
                return new KkSingleOfDoubleGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<Double>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfDoubleGetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDateTimeSetReply KkSingleOfDateTimeSet(RmContext context, KkSingleOfDateTimeSet param)
        {
            try
            {
                _keyStoreServer.SetSingleValue(param.StoreKey, param.ValueKey, param.Value);
                return new KkSingleOfDateTimeSetReply(true);
            }
            catch (Exception ex)
            {
                return new KkSingleOfDateTimeSetReply(ex.GetBaseException());
            }
        }

        public KkSingleOfDateTimeGetReply KkSingleOfDateTimeGet(RmContext context, KkSingleOfDateTimeGet param)
        {
            try
            {
                return new KkSingleOfDateTimeGetReply(true)
                {
                    Value = _keyStoreServer.GetSingleValue<DateTime>(param.StoreKey, param.ValueKey)
                };
            }
            catch (Exception ex)
            {
                return new KkSingleOfDateTimeGetReply(ex.GetBaseException());
            }
        }

        #endregion

        #region List Values.

        public KkListOfGuidAddReply KkListOfGuidAdd(RmContext context, KkListOfGuidAdd param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfGuidAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfGuidAddReply(ex.GetBaseException());
            }
        }

        public KkListOfGuidGetReply KkListOfGuidGet(RmContext context, KkListOfGuidGet param)
        {
            try
            {
                return new KkListOfGuidGetReply(true)
                {
                    List = _keyStoreServer.GetList<Guid>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfGuidGetReply(ex.GetBaseException());
            }
        }

        public KkListOfStringAddReply KkListOfStringAdd(RmContext context, KkListOfStringAdd param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfStringAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfStringAddReply(ex.GetBaseException());
            }
        }

        public KkListOfStringGetReply KkListOfStringGet(RmContext context, KkListOfStringGet param)
        {
            try
            {
                return new KkListOfStringGetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfStringGetReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32AddReply KkListOfInt32Add(RmContext context, KkListOfInt32Add param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt32AddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt32AddReply(ex.GetBaseException());
            }
        }

        public KkListOfInt32GetReply KkListOfInt32Get(RmContext context, KkListOfInt32Get param)
        {
            try
            {
                return new KkListOfInt32GetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt32GetReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64AddReply KkListOfInt64Add(RmContext context, KkListOfInt64Add param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfInt64AddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfInt64AddReply(ex.GetBaseException());
            }
        }

        public KkListOfInt64GetReply KkListOfInt64Get(RmContext context, KkListOfInt64Get param)
        {
            try
            {
                return new KkListOfInt64GetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfInt64GetReply(ex.GetBaseException());
            }
        }

        public KkListOfSingleAddReply KkListOfSingleAdd(RmContext context, KkListOfSingleAdd param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfSingleAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfSingleAddReply(ex.GetBaseException());
            }
        }

        public KkListOfSingleGetReply KkListOfSingleGet(RmContext context, KkListOfSingleGet param)
        {
            try
            {
                return new KkListOfSingleGetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfSingleGetReply(ex.GetBaseException());
            }
        }

        public KkListOfDoubleAddReply KkListOfDoubleAdd(RmContext context, KkListOfDoubleAdd param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDoubleAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDoubleAddReply(ex.GetBaseException());
            }
        }

        public KkListOfDoubleGetReply KkListOfDoubleGet(RmContext context, KkListOfDoubleGet param)
        {
            try
            {
                return new KkListOfDoubleGetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDoubleGetReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeAddReply KkListOfDateTimeAdd(RmContext context, KkListOfDateTimeAdd param)
        {
            try
            {
                _keyStoreServer.AddListValue(param.StoreKey, param.ListKey, param.ListValue);
                return new KkListOfDateTimeAddReply(true);
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeAddReply(ex.GetBaseException());
            }
        }

        public KkListOfDateTimeGetReply KkListOfDateTimeGet(RmContext context, KkListOfDateTimeGet param)
        {
            try
            {
                return new KkListOfDateTimeGetReply(true)
                {
                    List = _keyStoreServer.GetList<string>(param.StoreKey, param.ListKey)
                };
            }
            catch (Exception ex)
            {
                return new KkListOfDateTimeGetReply(ex.GetBaseException());
            }
        }

        #endregion
    }
}
