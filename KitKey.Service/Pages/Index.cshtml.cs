using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using NTDLS.KitKey.Server;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]

    public class IndexModel(ILogger<IndexModel> logger, KkServer mqServer) : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        public List<KkStoreDescriptor> Stores { get; private set; } = new();
        public KkServerDescriptor ServerConfig = new();
        public string ApplicationVersion { get; private set; } = string.Empty;

        public void OnGet()
        {
            ApplicationVersion = string.Join('.', (Assembly.GetExecutingAssembly()
                .GetName().Version?.ToString() ?? "0.0.0.0").Split('.').Take(3)); //Major.Minor.Patch

            try
            {
                ServerConfig = mqServer.GetConfiguration();
                Stores = mqServer.GetStores()?.OrderBy(o => o.StoreKey)?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
