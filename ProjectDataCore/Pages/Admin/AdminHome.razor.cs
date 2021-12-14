using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin;
public partial class AdminHome : ComponentBase
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
	public UserManager<DataCoreUser> UserManager { get; set; }
    [Inject]
    public DataCoreSignInManager SignInManager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	[CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

    private DataCoreUser NewUser { get; set; } = new();
    private string NewPassword { get; set; }

    private string Username { get; set; }
    private string Password { get; set; }

    private List<string> Errors { get; set; } = new();

    protected async Task CreateAccountAsync()
	{
        var res = await UserManager.CreateAsync(NewUser, NewPassword);

        Errors.Clear();
        if (!res.Succeeded)
        {
            foreach (var error in res.Errors)
                Errors.Add($"{error.Code} : {error.Description}");
        }
        else
        {
            NewUser = new();
            NewPassword = "";
        }

        StateHasChanged();
    }

    protected async Task LoginAsync()
	{
        var res = await SignInManager.PasswordSignInAsync(Username, Password, false, false);
        Errors.Clear();
        if (!res.Succeeded)
        {
            Errors.Add($"Lockout: {res.IsLockedOut}, Not Allowed: {res.IsNotAllowed}, TFA: {res.RequiresTwoFactor}");
        }
        else
        {
            Username = "";
            Password = "";
        }

        StateHasChanged();
    }

    protected async Task LogoutAsync() 
	{
        await SignInManager.SignOutAsync();
        StateHasChanged();
	}
}
