﻿using NTDLS.KitKey.Client;
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

            var _server = new KkServer(serverConfig);
            var _client = new KkClient();

            _server.Start(KkDefaults.DEFAULT_KEYSTORE_PORT);
            _client.Connect("localhost", KkDefaults.DEFAULT_KEYSTORE_PORT);

            _client.CreateStore(new KkStoreConfiguration("MyPersistentStore")
            {
                PersistenceScheme = CMqPersistenceScheme.Persistent
            });

            _client.CreateStore(new KkStoreConfiguration("MyEphemeralStore")
            {
                PersistenceScheme = CMqPersistenceScheme.Ephemeral
            });

            for (int i = 0; i < 100000; i++)
            {
                var randomKey = Guid.NewGuid().ToString().Substring(0, 4);
                var randomValue = Guid.NewGuid().ToString();

                _client.Set("MyPersistentStore", randomKey, randomValue);
                _client.Set("MyEphemeralStore", randomKey, randomValue);
            }

            Console.WriteLine("Press [enter] to stop.");
            Console.ReadLine();

            _client.Disconnect();

            _server.Stop();
        }
    }
}
