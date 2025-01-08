using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [AllowAnonymous]
    public class LogoutModel(ILogger<LogoutModel> logger) : BasePageModel
    {
        private readonly ILogger<LogoutModel> _logger = logger;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await HttpContext.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod()?.Name ?? string.Empty);
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Login");
        }
    }
}
