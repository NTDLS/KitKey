using Serilog;
using Topshelf;

namespace KitKey.Service
{
    internal class Program
    {
        static void Main()
        {
            // Load appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configure Serilog using the configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            HostFactory.Run(x =>
            {
                x.StartAutomatically();

                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                });

                x.Service<KeyStoreService>(s =>
                {
                    s.ConstructUsing(hostSettings => new KeyStoreService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("A low latency, high-performance, and reliable persistent or ephemeral key-value store over TCP/IP.");
                x.SetDisplayName("KitKey Key-Value Store");
                x.SetServiceName("KitKeyService");
            });
        }
    }
}
