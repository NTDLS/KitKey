using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload.ClientToServer;
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
        public delegate void OnExceptionEvent(KkClient client, string? storeName, Exception ex);

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
        public void StoreCreate(string storeName)
        {
            var result = _rmClient.Query(new KkStoreCreate(new KkStoreConfiguration(storeName))).Result;
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
        public void StoreDelete(string storeName)
        {
            var result = _rmClient.Query(new KkStoreDelete(storeName)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Removes all values from a key-store.
        /// </summary>
        public void StorePurge(string storeName)
        {
            var result = _rmClient.Query(new KkStorePurge(storeName)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Inserts or updates a value in a key-store.
        /// </summary>
        public void StringSet(string storeName, string key, string value)
        {
            var result = _rmClient.Query(new KkStringSet(storeName, key, value)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Gets a value from a key-store.
        /// </summary>
        public string? StringGet(string storeName, string key)
        {
            var result = _rmClient.Query(new KkStringGet(storeName, key)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Value;
        }

        /// <summary>
        /// Appends a value to a list key-store.
        /// </summary>
        public void ListAdd(string storeName, string key, string value)
        {
            var result = _rmClient.Query(new KkListAdd(storeName, key, value)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Gets a list from the key-store by its key.
        /// </summary>
        public Dictionary<Guid, string>? ListGet(string storeName, string key)
        {
            var result = _rmClient.Query(new KkListGet(storeName, key)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.List;
        }

        /// <summary>
        /// Deletes a value from a key-store.
        /// </summary>
        public void Delete(string storeName, string key)
        {
            var result = _rmClient.Query(new KkDelete(storeName, key)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
