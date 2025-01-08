using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Server.Management;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]

    public class StoresModel(ILogger<StoresModel> logger, KkServer mqServer) : BasePageModel
    {
        private readonly ILogger<StoresModel> _logger = logger;
        public List<KkStoreDescriptor> Stores { get; private set; } = new();

        public void OnGet()
        {
            try
            {
                Stores = mqServer.GetStores()?.OrderBy(o => o.StoreName)?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
