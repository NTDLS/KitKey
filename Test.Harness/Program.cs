using NTDLS.KitKey.Client;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Shared;

namespace Test.Harness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serverConfig = new KkServerConfiguration()
            {
                PersistencePath = Path.GetDirectoryName(Environment.ProcessPath)
            };

            var server = new KkServer(serverConfig);
            var client = new KkClient();

            server.Start(KkDefaults.DEFAULT_KEYSTORE_PORT);
            client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            client.CreateStore(new KkStoreConfiguration("MyPersistentStore")
            {
                PersistenceScheme = KkPersistenceScheme.Persistent,

            });

            client.CreateStore(new KkStoreConfiguration("MyEphemeralStore")
            {
                PersistenceScheme = KkPersistenceScheme.Ephemeral
            });

            for (int i = 0; i < 100000; i++)
            {
                var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var randomValue = Guid.NewGuid().ToString();

                client.Set("MyPersistentStore", randomKey, randomValue);
                client.Set("MyEphemeralStore", randomKey, randomValue);
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            client.Disconnect();

            server.Stop();
        }
    }
}
