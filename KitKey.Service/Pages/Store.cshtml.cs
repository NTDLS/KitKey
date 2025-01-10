using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Server.Management;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class StoreModel(ILogger<StoreModel> logger, KkClient mqServer) : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string StoreKey { get; set; } = string.Empty;

        private readonly ILogger<StoreModel> _logger = logger;
        public KkStoreDescriptor Store { get; private set; } = new();

        public void OnGet()
        {
            try
            {
                Store = mqServer.GetStores()?.Where(o => o.StoreKey.Equals(StoreKey, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
