using ProjectDataCore.Data.Services.Routing;

namespace ProjectDataCore.Components.Parts;

public class CustomComponentBase : ComponentBase
{
#pragma warning disable CS8618 // Injected services are not null.
    [Inject]
    public IScopedUserService ScopedUserService { get; set; }
    [Inject]
    public IPageEditService PageEditService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; }
    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }
    [CascadingParameter(Name = "RefreshRequest")]
    public Func<Task>? CallRefreshRequest { get; set; }
    [CascadingParameter(Name = "ScopeUserOverride")]
    public DataCoreUser? ScopeUserOverride { get; set; }
    #region User Scope
    protected List<DataCoreUser>? ScopedUsers { get; set; }

    protected void LoadScopedUser(Guid? scope = null)
    {
        if (ScopeUserOverride is not null)
            ScopedUsers = new() { ScopeUserOverride };
        else if (scope is not null)
            ScopedUsers = ScopedUserService.GetScopedUser(scope.Value);
        else if (ActiveUser is not null)
            ScopedUsers = new() { ActiveUser };
    }
    #endregion
}
