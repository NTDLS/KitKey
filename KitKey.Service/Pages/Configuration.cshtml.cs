using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class ConfigurationModel(ILogger<ConfigurationModel> logger) : BasePageModel
    {
        private readonly ILogger<ConfigurationModel> _logger = logger;

        [BindProperty]
        public ServiceConfiguration ServerConfig { get; set; } = new();

        public IActionResult OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Configs.PutServiceConfig(ServerConfig);
                    SuccessMessage = "Saved!<br />You will need to restart the service for these changes to take affect.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public void OnGet()
        {
            try
            {
                ServerConfig = Configs.GetServiceConfig();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
        }
    }
}
