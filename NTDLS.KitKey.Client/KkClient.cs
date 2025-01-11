using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload.Deletes;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle;
using NTDLS.KitKey.Shared.Payload.ListOf.ListOfString;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDateTime;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDouble;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfGuid;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfInt32;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfInt64;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfSingle;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfString;
using NTDLS.KitKey.Shared.Payload.Stores;
using NTDLS.ReliableMessaging;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace NTDLS.KitKey.Client
{
    /// <summary>
    /// Connects to a MessageServer then sends/received and processes notifications/queries.
    /// </summary>
    public class KkClient
    {
        private readonly RmClient _rmClient;
        private bool _explicitDisconnect = false;
        private readonly KkClientConfiguration _configuration;

        private string? _lastReconnectHost;
        private int _lastReconnectPort;
        private IPAddress? _lastReconnectIpAddress;

        /// <summary>
        /// Returns true if the client is connected.
        /// </summary>
        public bool IsConnected => _rmClient.IsConnected;

        /// <summary>
        /// Event used for server-to-client delivery notifications containing raw JSON.
        /// </summary>
        public delegate void OnConnectedEvent(KkClient client);

        /// <summary>
        /// Event used client connectivity notifications.
        /// </summary>
        public event OnConnectedEvent? OnConnected;

        /// <summary>
        /// Event used client connectivity notifications.
        /// </summary>
        public event OnConnectedEvent? OnDisconnected;

        /// <summary>
        /// Delegate used to notify of key-store client exceptions.
        /// </summary>
        public delegate void OnExceptionEvent(KkClient client, string? storeKey, Exception ex);

        /// <summary>
        /// Event used to notify of key-store client exceptions.
        /// </summary>
        public event OnExceptionEvent? OnException;

        /// <summary>
        /// Creates a new instance of the key-store client.
        /// </summary>
        public KkClient(KkClientConfiguration configuration)
        {
            _configuration = configuration;

            var rmConfiguration = new RmConfiguration()
            {
                AsynchronousQueryWaiting = configuration.AsynchronousAcknowledgment,
                InitialReceiveBufferSize = configuration.InitialReceiveBufferSize,
                MaxReceiveBufferSize = configuration.MaxReceiveBufferSize,
                QueryTimeout = configuration.QueryTimeout,
                ReceiveBufferGrowthRate = configuration.ReceiveBufferGrowthRate
            };

            _rmClient = new RmClient(rmConfiguration);
        }

        /// <summary>
        /// Creates a new instance of the key-store client.
        /// </summary>
        public KkClient()
        {
            _configuration = new KkClientConfiguration();
            _rmClient = new RmClient();

            _rmClient.OnConnected += RmClient_OnConnected;
            _rmClient.OnDisconnected += RmClient_OnDisconnected;
        }

        private void RmClient_OnConnected(RmContext context)
        {
            _explicitDisconnect = false;
            OnConnected?.Invoke(this);
        }

        private void RmClient_OnDisconnected(RmContext context)
        {
            OnDisconnected?.Invoke(this);

            if (_explicitDisconnect == false && _configuration.AutoReconnect)
            {
                new Thread((o) =>
                {
                    while (!_explicitDisconnect && !_rmClient.IsConnected)
                    {
                        try
                        {
                            if (_lastReconnectHost != null)
                            {
                                _rmClient.Connect(_lastReconnectHost, _lastReconnectPort);
                            }
                            else if (_lastReconnectIpAddress != null)
                            {
                                _rmClient.Connect(_lastReconnectIpAddress, _lastReconnectPort);
                            }
                            else
                            {
                                break; //What else can we do.
                            }
                        }
                        catch (Exception ex)
                        {
                            OnException?.Invoke(this, null, ex.GetBaseException());
                        }

                        Thread.Sleep(1000);
                    }
                }).Start();
            }
        }

        /// <summary>
        /// Connects the client to a key-store server.
        /// </summary>
        public void Connect(string hostName, int port)
        {
            _lastReconnectHost = hostName;
            _lastReconnectIpAddress = null;
            _lastReconnectPort = port;

            _explicitDisconnect = false;

            _rmClient.Connect(hostName, port);
        }

        /// <summary>
        /// Connects the client to a key-store server.
        /// </summary>
        public void Connect(IPAddress ipAddress, int port)
        {
            _lastReconnectHost = null;
            _lastReconnectIpAddress = ipAddress;
            _lastReconnectPort = port;

            _explicitDisconnect = false;

            _rmClient.Connect(ipAddress, port);
        }

        /// <summary>
        /// Connects the client to a key-store server.
        /// </summary>
        public void ConnectAsync(string hostName, int port)
        {
            new Thread(() =>
            {
                while (!_explicitDisconnect)
                {
                    try
                    {
                        Connect(hostName, port);
                        return;
                    }
                    catch
                    {
                        if (_configuration.AutoReconnect == false)
                        {
                            return;
                        }
                    }
                    Thread.Sleep(500);
                }
            }).Start();
        }

        /// <summary>
        /// Connects the client to a key-store server.
        /// </summary>
        public void ConnectAsync(IPAddress ipAddress, int port)
        {
            new Thread(() =>
            {
                while (!_explicitDisconnect)
                {
                    try
                    {
                        Connect(ipAddress, port);
                        return;
                    }
                    catch
                    {
                        if (_configuration.AutoReconnect == false)
                        {
                            return;
                        }
                    }
                    Thread.Sleep(500);
                }
            }).Start();
        }

        /// <summary>
        /// Disconnects the client from the key-store server.
        /// </summary>
        public void Disconnect(bool wait = false)
        {
            _explicitDisconnect = true;
            _rmClient.Disconnect(wait);
        }

        /// <summary>
        /// Creates a new key-store with a default configuration.
        /// </summary>
        public void StoreCreate(string storeKey)
           => _rmClient.Query(new KkStoreCreate(new KkStoreConfiguration(storeKey))).Result.EnsureSuccessful();

        /// <summary>
        /// Creates a new key-store with a custom configuration.
        /// </summary>
        public void CreateStore(KkStoreConfiguration storeConfiguration)
            => _rmClient.Query(new KkStoreCreate(storeConfiguration)).Result.EnsureSuccessful();

        /// <summary>
        /// Deletes a key-store and all its values.
        /// </summary>
        public void DeleteStore(string storeKey)
            => _rmClient.Query(new KkStoreDelete(storeKey)).Result.EnsureSuccessful();

        /// <summary>
        /// Removes all values from a key-store.
        /// </summary>
        public void StorePurge(string storeKey)
            => _rmClient.Query(new KkStorePurge(storeKey)).Result.EnsureSuccessful();

        /// <summary>
        /// Inserts or updates a value in the given key-value store.
        /// </summary>
        public void Set<T>(string storeKey, string valueKey, T value)
        {
            if (value == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            if (value is string stringValue)
                _rmClient.Query(new KkSingleOfStringSet(storeKey, valueKey, stringValue)).Result.EnsureSuccessful();
            else if (value is Guid guidValue)
                _rmClient.Query(new KkSingleOfGuidSet(storeKey, valueKey, guidValue)).Result.EnsureSuccessful();
            else if (value is Int32 int32Value)
                _rmClient.Query(new KkSingleOfInt32Set(storeKey, valueKey, int32Value)).Result.EnsureSuccessful();
            else if (value is Int64 int64Value)
                _rmClient.Query(new KkSingleOfInt64Set(storeKey, valueKey, int64Value)).Result.EnsureSuccessful();
            else if (value is Single singleValue)
                _rmClient.Query(new KkSingleOfSingleSet(storeKey, valueKey, singleValue)).Result.EnsureSuccessful();
            else if (value is Double doubleValue)
                _rmClient.Query(new KkSingleOfDoubleSet(storeKey, valueKey, doubleValue)).Result.EnsureSuccessful();
            else if (value is DateTime dateTimeValue)
                _rmClient.Query(new KkSingleOfDateTimeSet(storeKey, valueKey, dateTimeValue)).Result.EnsureSuccessful();
            else
                throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets a single value from the given key-value store.
        /// </summary>
        public T? Get<T>(string storeKey, string valueKey)
        {
            var genericType = typeof(T);
            genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

            if (genericType == typeof(string))
                return (T?)(object?)_rmClient.Query(new KkSingleOfStringGet(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(Guid))
                return (T?)(object?)_rmClient.Query(new KkSingleOfGuidGet(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(Int32))
                return (T?)(object?)_rmClient.Query(new KkSingleOfInt32Get(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(Int64))
                return (T?)(object?)_rmClient.Query(new KkSingleOfInt64Get(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(Single))
                return (T?)(object?)_rmClient.Query(new KkSingleOfSingleGet(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(Double))
                return (T?)(object?)_rmClient.Query(new KkSingleOfDoubleGet(storeKey, valueKey)).Result.EnsureSuccessful().Value;
            else if (genericType == typeof(DateTime))
                return (T?)(object?)_rmClient.Query(new KkSingleOfDateTimeGet(storeKey, valueKey)).Result.EnsureSuccessful().Value;

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets a single value from the given key-value store, returns true if the key was found.
        /// </summary>
        public bool TryGet<T>(string storeKey, string valueKey, [NotNullWhen(true)] out T? outValue)
        {
            outValue = Get<T?>(storeKey, valueKey);
            return outValue != null;
        }

        /// <summary>
        /// Appends a value to the list in the given key-value store.
        /// </summary>
        public void PushLast<T>(string storeKey, string listKey, T listValue)
        {
            if (listValue == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            if (listValue is string stringValue)
                _rmClient.Query(new KkListOfStringPushLast(storeKey, listKey, stringValue)).Result.EnsureSuccessful();
            else if (listValue is Guid guidValue)
                _rmClient.Query(new KkListOfGuidPushLast(storeKey, listKey, guidValue)).Result.EnsureSuccessful();
            else if (listValue is Int32 int32Value)
                _rmClient.Query(new KkListOfInt32PushLast(storeKey, listKey, int32Value)).Result.EnsureSuccessful();
            else if (listValue is Int64 int64Value)
                _rmClient.Query(new KkListOfInt64PushLast(storeKey, listKey, int64Value)).Result.EnsureSuccessful();
            else if (listValue is Single singleValue)
                _rmClient.Query(new KkListOfSinglePushLast(storeKey, listKey, singleValue)).Result.EnsureSuccessful();
            else if (listValue is Double doubleValue)
                _rmClient.Query(new KkListOfDoublePushLast(storeKey, listKey, doubleValue)).Result.EnsureSuccessful();
            else if (listValue is DateTime dateTimeValue)
                _rmClient.Query(new KkListOfDateTimePushLast(storeKey, listKey, dateTimeValue)).Result.EnsureSuccessful();
            else
                throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Prepends a value to the list in the given key-value store.
        /// </summary>
        public void PushFirst<T>(string storeKey, string listKey, T listValue)
        {
            if (listValue == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            if (listValue is string stringValue)
                _rmClient.Query(new KkListOfStringPushFirst(storeKey, listKey, stringValue)).Result.EnsureSuccessful();
            else if (listValue is Guid guidValue)
                _rmClient.Query(new KkListOfGuidPushFirst(storeKey, listKey, guidValue)).Result.EnsureSuccessful();
            else if (listValue is Int32 int32Value)
                _rmClient.Query(new KkListOfInt32PushFirst(storeKey, listKey, int32Value)).Result.EnsureSuccessful();
            else if (listValue is Int64 int64Value)
                _rmClient.Query(new KkListOfInt64PushFirst(storeKey, listKey, int64Value)).Result.EnsureSuccessful();
            else if (listValue is Single singleValue)
                _rmClient.Query(new KkListOfSinglePushFirst(storeKey, listKey, singleValue)).Result.EnsureSuccessful();
            else if (listValue is Double doubleValue)
                _rmClient.Query(new KkListOfDoublePushFirst(storeKey, listKey, doubleValue)).Result.EnsureSuccessful();
            else if (listValue is DateTime dateTimeValue)
                _rmClient.Query(new KkListOfDateTimePushFirst(storeKey, listKey, dateTimeValue)).Result.EnsureSuccessful();
            else
                throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets the full list from the given key-value store.
        /// </summary>
        public List<KkListItem<T>>? GetList<T>(string storeKey, string listKey)
        {
            var genericType = typeof(T);
            genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

            if (genericType == typeof(string))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfStringGetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(Guid))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfGuidGetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(Int32))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfInt32GetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(Int64))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfInt64GetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(Single))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfSingleGetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(Double))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfDoubleGetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());
            else if (genericType == typeof(DateTime))
                return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfDateTimeGetAll(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets the full list from the given key-value store, returns true if the key was found.
        /// </summary>
        public bool TryGetList<T>(string storeKey, string valueKey, [NotNullWhen(true)] out List<KkListItem<T>>? outValue)
        {
            outValue = GetList<T>(storeKey, valueKey);
            return outValue != null;
        }

        /// <summary>
        /// Gets the first item in a list from the given key-value store.
        /// </summary>
        public KkListItem<T>? GetFirst<T>(string storeKey, string listKey)
        {
            var genericType = typeof(T);
            genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

            if (genericType == typeof(string))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfStringGetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Guid))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfGuidGetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Int32))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfInt32GetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Int64))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfInt64GetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Single))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfSingleGetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Double))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfDoubleGetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(DateTime))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfDateTimeGetFirst(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets the first item in a list from the given key-value store, returns true if the key was found.
        /// </summary>
        public bool TryGetFirst<T>(string storeKey, string valueKey, [NotNullWhen(true)] out KkListItem<T>? outValue)
        {
            outValue = GetFirst<T>(storeKey, valueKey);
            return outValue != null;
        }

        /// <summary>
        /// Gets the last item in a list from the given key-value store.
        /// </summary>
        public KkListItem<T>? GetLast<T>(string storeKey, string listKey)
        {
            var genericType = typeof(T);
            genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

            if (genericType == typeof(string))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfStringGetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Guid))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfGuidGetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Int32))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfInt32GetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Int64))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfInt64GetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Single))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfSingleGetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(Double))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfDoubleGetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
            else if (genericType == typeof(DateTime))
                return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfDateTimeGetLast(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Gets the last item in a list from the given key-value store, returns true if the key was found.
        /// </summary>
        public bool TryGetLast<T>(string storeKey, string valueKey, [NotNullWhen(true)] out KkListItem<T>? outValue)
        {
            outValue = GetFirst<T>(storeKey, valueKey);
            return outValue != null;
        }

        /// <summary>
        /// Removes a key from a key-store of any type.
        /// </summary>
        public void Remove(string storeKey, string valueKey)
        {
            var result = _rmClient.Query(new KkRemoveKey(storeKey, valueKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Removes a list value from a list-of-values key-store by the list value id.
        /// </summary>
        public void RemoveListItemByKey(string storeKey, string listKey, Guid listItemKey)
        {
            var result = _rmClient.Query(new KkRemoveListItemByKey(storeKey, listKey, listItemKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Clears the cache for a single key-store.
        /// </summary>
        public void FlushCache(string storeKey)
        {
            var result = _rmClient.Query(new KkStoreFlushCache(storeKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Clears the cache for a all key-stores.
        /// </summary>
        public void FlushCache()
        {
            var result = _rmClient.Query(new KkStoreFlushAllCaches()).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
