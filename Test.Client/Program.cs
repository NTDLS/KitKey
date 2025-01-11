using NTDLS.KitKey.Client;
using NTDLS.KitKey.Shared;

namespace Test.Client
{
    internal class Program
    {
        static void Main()
        {
            for (int i = 0; i < 2; i++)
            {
                new Thread(ListTestsThreadProc).Start();
                new Thread(RandomInsertAndGetThreadProc).Start();
            }
        }

        private static void ListTestsThreadProc()
        {
            var _client = new KkClient();

            _client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            _client.CreateStore(new KkStoreConfiguration("MyPersistentListStore")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfStrings
            });

            var rand = new Random();

            for (int i = 0; i < 10000; i++)
            {
                var randomKey1 = Guid.NewGuid().ToString().Substring(0, 2);
                var randomKey2 = Guid.NewGuid().ToString().Substring(0, 2);

                var list = _client.GetList<string>("MyPersistentListStore", $"MyKey:{randomKey1}");

                _client.PushLast("MyPersistentListStore", $"MyKey:{randomKey2}", $"Item #{i:n0}");
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();
        }

        private static void RandomInsertAndGetThreadProc()
        {
            var _client = new KkClient();

            _client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            _client.CreateStore(new KkStoreConfiguration("MyPersistentStore")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent
            });

            _client.CreateStore(new KkStoreConfiguration("MyEphemeralStore")
            {
                PersistenceScheme = KkPersistenceScheme.Ephemeral
            });

            var rand = new Random();

            for (int i = 0; i < 100000; i++)
            {
                var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var randomValue = Guid.NewGuid().ToString();

                _client.Set("MyPersistentStore", randomKey, randomValue);
                _client.Set("MyEphemeralStore", randomKey, randomValue);

                randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var a = _client.Get<string>("MyPersistentStore", randomKey);
                var b = _client.Get<string>("MyEphemeralStore", randomKey);

                if (rand.Next(0, 100) > 75)
                {
                    randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                    _client.Remove("MyPersistentStore", randomKey);
                    _client.Remove("MyEphemeralStore", randomKey);
                }
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();
        }
    }
}
