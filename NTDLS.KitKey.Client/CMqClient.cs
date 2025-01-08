using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload.ClientToServer;
using NTDLS.ReliableMessaging;
using System.Net;

namespace NTDLS.KitKey.Client
{
    /// <summary>
    /// Connects to a MessageServer then sends/received and processes notifications/queries.
    /// </summary>
    public class CMqClient
    {
        private readonly RmClient _rmClient;
        private bool _explicitDisconnect = false;
        private readonly CMqClientConfiguration _configuration;

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
        public delegate void OnConnectedEvent(CMqClient client);

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
        public delegate void OnExceptionEvent(CMqClient client, string? queueName, Exception ex);

        /// <summary>
        /// Event used to notify of key-store client exceptions.
        /// </summary>
        public event OnExceptionEvent? OnException;

        /// <summary>
        /// Creates a new instance of the key-store client.
        /// </summary>
        public CMqClient(CMqClientConfiguration configuration)
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
        public CMqClient()
        {
            _configuration = new CMqClientConfiguration();
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
        /// Creates a new key-store.
        /// </summary>
        public void CreateStore(string queueName)
        {
            var result = _rmClient.Query(new CMqCreateStore(new CMqStoreConfiguration(queueName))).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Creates a new key-store.
        /// </summary>
        public void CreateStore(CMqStoreConfiguration queueConfiguration)
        {
            var result = _rmClient.Query(new CMqCreateStore(queueConfiguration)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Deletes a key-store and all its values.
        /// </summary>
        public void DeleteStore(string queueName)
        {
            var result = _rmClient.Query(new CMqDeleteStore(queueName)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Removes all values from a key-store.
        /// </summary>
        public void PurgeStore(string queueName)
        {
            var result = _rmClient.Query(new CMqPurgeStore(queueName)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Inserts or updates a value in a key-store.
        /// </summary>
        public void Upsert(string storeName, string key, string value)
        {
            var result = _rmClient.Query(new CMqUpsert(storeName, key, value)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Gets a value from a key-store.
        /// </summary>
        public string? Upsert(string storeName, string key)
        {
            var result = _rmClient.Query(new CMqGet(storeName, key)).Result;
            if (result.IsSuccess == false)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Value;
        }
    }
}
