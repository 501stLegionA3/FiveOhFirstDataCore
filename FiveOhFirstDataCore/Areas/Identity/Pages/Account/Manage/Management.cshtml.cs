using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account.Manage
{
    public class ManagementModel : PageModel
    {
        private readonly UserManager<Trooper> _userManager;
        private readonly SignInManager<Trooper> _signInManager;

        public ManagementModel(
            UserManager<Trooper> userManager,
            SignInManager<Trooper> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Enable Permissions View?")]
            public bool PermissionsView { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input is null) Input = new();

            Input.PermissionsView = user.PermissionsView;

            return Page();
        }

        public async Task<IActionResult> OnPostSaveChangesAsync()
		{
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.PermissionsView = Input.PermissionsView;

            await _userManager.UpdateAsync(user);

            StatusMessage = "Changes saved.";

            return await OnGetAsync();
        }
    }
}
