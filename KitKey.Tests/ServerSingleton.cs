using NTDLS.KitKey.Server;

namespace KitKey.Tests
{
    public class ServerSingleton
    {
        private static int _referenceCount = 0;
        private static readonly object _lock = new object();
        private static KkServer? _server;

        public static KkServer GetSingleInstance()
        {
            _referenceCount++;

            if (_server == null)
            {
                lock (_lock)
                {
                    _server ??= CreateNewInstance();
                }
            }

            return _server;
        }

        private static KkServer CreateNewInstance()
        {
            try
            {

                Directory.Delete(Constants.ROOT_PATH, true);
            }
            catch
            {
            }
            Directory.CreateDirectory(Constants.ROOT_PATH);

            bool rootDirectoryFreshlyCreated = Directory.Exists(Constants.ROOT_PATH);

            var config = new KkServerConfiguration()
            {
                PersistencePath = Constants.ROOT_PATH
            };

            var server = new KkServer(config);
            server.OnLog += Server_OnLog;
            server.Start(Constants.LISTEN_PORT);

            return server;
        }

        private static void Server_OnLog(KkServer server, NTDLS.KitKey.Shared.KkErrorLevel errorLevel, string message, Exception? ex = null)
        {
            Console.WriteLine($"{errorLevel}: {message}");
        }

        public static void Dereference()
        {
            _referenceCount--;

            if (_referenceCount == 0)
            {
                lock (_lock)
                {
                    _server?.Stop();
                    _server = null;
                }
            }
        }
    }
}
