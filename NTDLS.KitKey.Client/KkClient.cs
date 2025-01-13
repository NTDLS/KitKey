using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload.Deletes;
using NTDLS.KitKey.Shared.Payload.ListOf;
using NTDLS.KitKey.Shared.Payload.SingleOf;
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
        public void CreateStore(string storeKey)
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
        public void PurgeStore(string storeKey)
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

            _rmClient.Query(new KkSetSingleOf<T>(storeKey, valueKey, value)).Result.EnsureSuccessful();
        }

        /// <summary>
        /// Gets a single value from the given key-value store.
        /// </summary>
        public T? Get<T>(string storeKey, string valueKey)
        {
            var genericType = typeof(T);
            genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

            return (T?)(object?)_rmClient.Query(new KkGetSingleOf<T>(storeKey, valueKey)).Result.EnsureSuccessful().Value;
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

            _rmClient.Query(new KkListOfPushLast<T>(storeKey, listKey, listValue)).Result.EnsureSuccessful();
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

            _rmClient.Query(new KkListOfPushFirst<T>(storeKey, listKey, listValue)).Result.EnsureSuccessful();
        }

        /// <summary>
        /// Gets the full list from the given key-value store.
        /// </summary>
        public List<KkListItem<T>>? GetList<T>(string storeKey, string listKey)
        {
            return (List<KkListItem<T>>?)(object)(_rmClient.Query(new KkListOfGetAll<T>(storeKey, listKey)).Result.EnsureSuccessful()?.List ?? new());

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
            return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfGetFirst<T>(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
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
            return (KkListItem<T>?)(object)(_rmClient.Query(new KkListOfGetLast<T>(storeKey, listKey)).Result.EnsureSuccessful()?.Value ?? new());
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
