using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class StoreDeleteModel(ILogger<StoreDeleteModel> logger, KkServer mqServer) : BasePageModel
    {
        private readonly ILogger<StoreDeleteModel> _logger = logger;

        public string? RedirectURL { get; set; }

        [BindProperty]
        public string StoreName { get; set; } = string.Empty;

        [BindProperty]
        public string? UserSelection { get; set; }

        public IActionResult OnPost()
        {
            RedirectURL = $"/Stores";

            try
            {
                if (UserSelection?.Equals("true") == true)
                {
                    mqServer.StoreDelete(StoreName);
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
