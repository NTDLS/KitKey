using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf;
using NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf;
using NTDLS.ReliableMessaging;
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
        {
            var result = _rmClient.Query(new KkStoreCreate(new KkStoreConfiguration(storeKey))).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Creates a new key-store with a custom configuration.
        /// </summary>
        public void StoreCreate(KkStoreConfiguration storeConfiguration)
        {
            var result = _rmClient.Query(new KkStoreCreate(storeConfiguration)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Deletes a key-store and all its values.
        /// </summary>
        public void StoreDelete(string storeKey)
        {
            var result = _rmClient.Query(new KkStoreDelete(storeKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Removes all values from a key-store.
        /// </summary>
        public void StorePurge(string storeKey)
        {
            var result = _rmClient.Query(new KkStorePurge(storeKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Inserts or updates a value in a key-store.
        /// </summary>
        public void Set<T>(string storeKey, string valueKey, T value)
        {
            if (value is string stringValue)
            {
                var result = _rmClient.Query(new KkSingleOfStringSet(storeKey, valueKey, stringValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (value is Int32 int32Value)
            {
                var result = _rmClient.Query(new KkSingleOfInt32Set(storeKey, valueKey, int32Value)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (value is Int64 int64Value)
            {
                var result = _rmClient.Query(new KkSingleOfInt64Set(storeKey, valueKey, int64Value)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (value is Single singleValue)
            {
                var result = _rmClient.Query(new KkSingleOfSingleSet(storeKey, valueKey, singleValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (value is Double doubleValue)
            {
                var result = _rmClient.Query(new KkSingleOfDoubleSet(storeKey, valueKey, doubleValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (value is DateTime dateTimeValue)
            {
                var result = _rmClient.Query(new KkSingleOfDateTimeSet(storeKey, valueKey, dateTimeValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else
            {
                throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
            }
        }

        /// <summary>
        /// Gets a value from a key-store.
        /// </summary>
        public T? Get<T>(string storeKey, string valueKey)
        {
            var genericType = typeof(T);

            if (genericType == typeof(string))
            {
                var result = _rmClient.Query(new KkSingleOfStringGet(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }
            else if (genericType == typeof(Int32))
            {
                var result = _rmClient.Query(new KkSingleOfInt32Get(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }
            else if (genericType == typeof(Int64))
            {
                var result = _rmClient.Query(new KkSingleOfInt64Get(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }
            else if (genericType == typeof(Single))
            {
                var result = _rmClient.Query(new KkSingleOfSingleGet(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }
            else if (genericType == typeof(Double))
            {
                var result = _rmClient.Query(new KkSingleOfDoubleGet(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }
            else if (genericType == typeof(DateTime))
            {
                var result = _rmClient.Query(new KkSingleOfDateTimeGet(storeKey, valueKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return (T?)(object?)result.Value;
            }

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Appends a value to a list key-store.
        /// </summary>
        public void AddToList<T>(string storeKey, string listKey, T listValue)
        {
            if (listValue is string stringValue)
            {
                var result = _rmClient.Query(new KkListOfStringAdd(storeKey, listKey, stringValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (listValue is Int32 int32Value)
            {
                var result = _rmClient.Query(new KkListOfInt32Add(storeKey, listKey, int32Value)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (listValue is Int64 int64Value)
            {
                var result = _rmClient.Query(new KkListOfInt64Add(storeKey, listKey, int64Value)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (listValue is Single singleValue)
            {
                var result = _rmClient.Query(new KkListOfSingleAdd(storeKey, listKey, singleValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (listValue is Double doubleValue)
            {
                var result = _rmClient.Query(new KkListOfDoubleAdd(storeKey, listKey, doubleValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else if (listValue is DateTime dateTimeValue)
            {
                var result = _rmClient.Query(new KkListOfDateTimeAdd(storeKey, listKey, dateTimeValue)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
            }
            else
            {
                throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
            }
        }

        /// <summary>
        /// Gets a list from the key-store by its key.
        /// </summary>
        public Dictionary<Guid, T>? GetList<T>(string storeKey, string listKey)
        {
            var genericType = typeof(T);

            if (genericType == typeof(string))
            {
                var result = _rmClient.Query(new KkListOfStringGet(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }
            else if (genericType == typeof(Int32))
            {
                var result = _rmClient.Query(new KkListOfInt32Get(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }
            else if (genericType == typeof(Int64))
            {
                var result = _rmClient.Query(new KkListOfInt64Get(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }
            else if (genericType == typeof(Single))
            {
                var result = _rmClient.Query(new KkListOfSingleGet(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }
            else if (genericType == typeof(Double))
            {
                var result = _rmClient.Query(new KkListOfDoubleGet(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }
            else if (genericType == typeof(DateTime))
            {
                var result = _rmClient.Query(new KkListOfDateTimeGet(storeKey, listKey)).Result;
                if (result.IsSuccess == false)
                {
                    throw new Exception(result.ErrorMessage);
                }
                return result.List == null ? null : (Dictionary<Guid, T>?)(object)result.List;
            }

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        /// <summary>
        /// Deletes a value from a key-store.
        /// </summary>
        public void Delete(string storeKey, string valueKey)
        {
            var result = _rmClient.Query(new KkDeleteKey(storeKey, valueKey)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
