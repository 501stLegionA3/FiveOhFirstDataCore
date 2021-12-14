using ProjectDataCore.Data.Services.User;

namespace ProjectDataCore.Shared;

public partial class MainLayout : LayoutComponentBase
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IUserService UserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter]
    public Task<AuthenticationState>? AuthStateTask { get; set; }
    /// <summary>
    /// The currently signed in user.
    /// </summary>
    public DataCoreUser? ActiveUser { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (AuthStateTask is not null)
        {
            var user = (await AuthStateTask).User;

            ActiveUser = await UserService.GetUserFromClaimsPrinciaplAsync(user);
        }
    }
}
