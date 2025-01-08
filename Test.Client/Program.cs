using NTDLS.KitKey.Client;
using NTDLS.KitKey.Shared;

namespace Test.Client
{
    internal class Program
    {
        static void Main()
        {
            for (int i = 0; i < 16; i++)
            {
                new Thread(ThreadProc).Start();
            }
        }

        private static void ThreadProc()
        {
            var _client = new KkClient();

            _client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            _client.CreateStore(new KkStoreConfiguration("MyPersistentStore")
            {
                PersistenceScheme = CMqPersistenceScheme.Persistent
            });

            _client.CreateStore(new KkStoreConfiguration("MyEphemeralStore")
            {
                PersistenceScheme = CMqPersistenceScheme.Ephemeral
            });

            var rand = new Random();

            for (int i = 0; i < 100000; i++)
            {
                var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var randomValue = Guid.NewGuid().ToString();

                _client.Set("MyPersistentStore", randomKey, randomValue);
                _client.Set("MyEphemeralStore", randomKey, randomValue);

                randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                _ = _client.Get("MyPersistentStore", randomKey);
                _ = _client.Get("MyEphemeralStore", randomKey);

                if (rand.Next(0, 100) > 75)
                {
                    randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                    _client.Delete("MyPersistentStore", randomKey);
                    _client.Delete("MyEphemeralStore", randomKey);
                }
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();
        }
    }
}
