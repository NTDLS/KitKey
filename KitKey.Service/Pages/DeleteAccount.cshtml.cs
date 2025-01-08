using KitKey.Service.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace KitKey.Service.Pages
{
    [Authorize]
    public class DeleteAccountModel(ILogger<DeleteAccountModel> logger) : BasePageModel
    {
        private readonly ILogger<DeleteAccountModel> _logger = logger;

        public string? RedirectURL { get; set; }

        [BindProperty]
        public Guid? AccountId { get; set; }

        [BindProperty]
        public string? UserSelection { get; set; }

        public IActionResult OnPost()
        {
            RedirectURL = $"/Accounts";

            try
            {
                if (UserSelection?.Equals("true") == true)
                {
                    var accounts = Configs.GetAccounts();
                    accounts.RemoveAll(o => o.Id.Equals(AccountId));
                    Configs.PutAccounts(accounts);
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
