using NTDLS.KitKey.Server;

namespace KitKey.Tests
{
    public class ServerFixture : IDisposable
    {
        public KkClient Server { get; private set; }

        public ServerFixture()
        {
            Server = ServerSingleton.GetSingleInstance();
        }

        public void Dispose()
        {
            ServerSingleton.Dereference();
            GC.SuppressFinalize(this);
        }
    }
}
