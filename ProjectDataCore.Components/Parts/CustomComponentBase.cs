using ProjectDataCore.Data.Services.Routing;

namespace ProjectDataCore.Components.Parts;

public class CustomComponentBase : ComponentBase
{
#pragma warning disable CS8618 // Injected services are not null.
    [Inject]
    public IScopedUserService ScopedUserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; }
    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

    #region User Scope
    protected DataCoreUser? ScopedUser { get; set; }

    protected void LoadScopedUserAsync(Guid? scope = null)
    {
        if (scope is not null)
            ScopedUser = ScopedUserService.GetScopedUser(scope.Value);
        else
            ScopedUser = ActiveUser;
    }
    #endregion
}
