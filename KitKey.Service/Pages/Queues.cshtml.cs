using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Server.Management;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]

    public class QueuesModel(ILogger<QueuesModel> logger, CMqServer mqServer) : BasePageModel
    {
        private readonly ILogger<QueuesModel> _logger = logger;
        public List<CMqStoreDescriptor> Queues { get; private set; } = new();

        public void OnGet()
        {
            try
            {
                Queues = mqServer.GetStores()?.OrderBy(o => o.StoreName)?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
