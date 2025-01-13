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

            _client.CreateStore(new KkStoreConfiguration("MyPersistentListStore.Strings")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfStrings
            });

            _client.CreateStore(new KkStoreConfiguration("MyPersistentListStore.Int32s")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.ListOfInt32s
            });

            var rand = new Random();

            for (int i = 0; i < 10000; i++)
            {
                var randomKey1 = Guid.NewGuid().ToString().Substring(0, 2);
                var randomKey2 = Guid.NewGuid().ToString().Substring(0, 2);

                var list = _client.GetList<string>("MyPersistentListStore", $"MyKey:{randomKey1}");

                _client.PushLast("MyPersistentListStore.Strings", $"MyKey:{randomKey2}", $"Item #{i:n0}");
                _client.PushLast("MyPersistentListStore.Int32s", $"MyKey:{randomKey2}", i);
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();
        }

        private static void RandomInsertAndGetThreadProc()
        {
            var _client = new KkClient();

            _client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            _client.CreateStore(new KkStoreConfiguration("MyPersistentStore.String")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent,
                ValueType = KkValueType.String
            });

            _client.CreateStore(new KkStoreConfiguration("MyEphemeralStore.Int32")
            {
                PersistenceScheme = KkPersistenceScheme.Ephemeral,
                ValueType = KkValueType.Int32
            });

            var rand = new Random();

            for (int i = 0; i < 100000; i++)
            {
                var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var randomValue = Guid.NewGuid().ToString();

                _client.Set("MyPersistentStore.String", randomKey, randomValue);
                _client.Set("MyEphemeralStore.Int32", randomKey, i);

                randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var a = _client.Get<string>("MyPersistentStore.String", randomKey);
                var b = _client.Get<int>("MyEphemeralStore.Int32", randomKey);

                if (rand.Next(0, 100) > 75)
                {
                    randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                    _client.Remove("MyPersistentStore.String", randomKey);
                    _client.Remove("MyEphemeralStore.Int32", randomKey);
                }
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();
        }
    }
}
