using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Server.Management;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class QueueModel(ILogger<QueueModel> logger, CMqServer mqServer) : BasePageModel
    {
        [BindProperty(SupportsGet = true)]
        public string QueueName { get; set; } = string.Empty;

        private readonly ILogger<QueueModel> _logger = logger;
        public CMqStoreDescriptor Store { get; private set; } = new();

        public void OnGet()
        {
            try
            {
                Store = mqServer.GetStores()?.Where(o => o.StoreName.Equals(QueueName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
