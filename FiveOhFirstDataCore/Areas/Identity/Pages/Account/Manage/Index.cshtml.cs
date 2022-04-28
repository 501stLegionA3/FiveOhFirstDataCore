using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Trooper> _userManager;
        private readonly SignInManager<Trooper> _signInManager;

        public IndexModel(
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
            [Display(Name = "Account Username")]
            public string Username { get; set; }
            [Display(Name = "Trooper Name")]
            public string NickName { get; set; }
            [Display(Name = "Birth Number")]
            public int TrooperNumber { get; set; }
        }

        private async Task LoadAsync(Trooper user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            if (Input is null) Input = new();

            Input.Username = userName;
            Input.NickName = user.NickName;
            Input.TrooperNumber = user.BirthNumber;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
