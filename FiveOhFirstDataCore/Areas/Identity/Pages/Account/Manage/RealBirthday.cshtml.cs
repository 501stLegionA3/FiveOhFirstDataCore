using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account.Manage
{
    public partial class RealBirthdayModel : PageModel
    {
        private readonly UserManager<Trooper> _userManager;
        private readonly SignInManager<Trooper> _signInManager;

        public RealBirthdayModel(
            UserManager<Trooper> userManager,
            SignInManager<Trooper> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; } = "";

        [Display(Name = "Current Birthday")]
        public DateTime RealBirthday { get; set; }


        public Boolean ShowBirthday { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.DateTime)]
            [Display(Name = "Birthday")]
            public DateTime NewBirthday { get; set; }

            [Display(Name = "Show Birthday?")]
            public Boolean NewShowBirthday { get; set; }
        }

        private async Task LoadAsync(Trooper user)
        {
            RealBirthday = user.RealBirthday;
            ShowBirthday = user.ShowBirthday;

            Input = new InputModel
            {
                NewBirthday = RealBirthday,
                NewShowBirthday = ShowBirthday
            };

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.NewBirthday.CompareTo(DateTime.Now.Subtract(TimeSpan.FromDays(5110))) > 0)
            {
                StatusMessage = "Your Birtday implies you are too young to be in the unit please select a different date.";
                return RedirectToPage();
            }

            user.RealBirthday = Input.NewBirthday;
            user.ShowBirthday = Input.NewShowBirthday;
            var changeBirthdayResult = await _userManager.UpdateAsync(user);
            if (!changeBirthdayResult.Succeeded)
            {
                foreach (var error in changeBirthdayResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            StatusMessage = "Your Birthday has been changed.";

            return RedirectToPage();
        }
    }
}
