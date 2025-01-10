using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class StorePurgeModel(ILogger<StorePurgeModel> logger, KkClient mqServer) : BasePageModel
    {
        private readonly ILogger<StorePurgeModel> _logger = logger;

        public string? RedirectURL { get; set; }

        [BindProperty]
        public string StoreKey { get; set; } = string.Empty;

        [BindProperty]
        public string? UserSelection { get; set; }

        public IActionResult OnPost()
        {
            RedirectURL = $"/Store/{StoreKey}";

            try
            {
                if (UserSelection?.Equals("true") == true)
                {
                    mqServer.StorePurge(StoreKey);
                }
                else
                {
                    return Redirect(RedirectURL);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
