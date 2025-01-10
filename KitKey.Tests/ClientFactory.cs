using NTDLS.KitKey.Client;

namespace KitKey.Tests
{
    public class ClientFactory
    {
        public static KkClient CreateAndConnect()
        {
            var client = new KkClient();

            client.OnException += Client_OnException;
            client.OnConnected += Client_OnConnected;
            client.OnDisconnected += Client_OnDisconnected;

            client.Connect(Constants.HOST_NAME, Constants.LISTEN_PORT);

            return client;
        }

        private static void Client_OnDisconnected(NTDLS.KitKey.Client.KkClient client)
        {
            Console.WriteLine("Client disconnected.");
        }

        private static void Client_OnConnected(NTDLS.KitKey.Client.KkClient client)
        {
            Console.WriteLine("Client connected.");
        }

        private static void Client_OnException(NTDLS.KitKey.Client.KkClient client, string? storeKey, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
