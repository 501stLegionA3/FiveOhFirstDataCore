using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Scope;
public class UserScope : DataObject<Guid>
{
    public CustomPageSettings Page { get; set; }
    public Guid PageId { get; set; }

    public List<PageComponentSettingsBase> ScopeProviders { get; set; } = new();
    public List<PageComponentSettingsBase> ScopeListeners { get; set; } = new();

    public bool IncludeLocalUser { get; set; } = false;
    public string DisplayName { get; set; } = "Unnamed User Scope";

    #region Non-Database Parameters
    private List<DataCoreUser> UsersInScope { get; init; } = new();
    #endregion

    public void RegisterUser(DataCoreUser user)
        => UsersInScope.Add(user);

    public bool RemoveUser(DataCoreUser user)
        => UsersInScope.Remove(user);

    public DataCoreUser? GetSingleUser()
        => UsersInScope.FirstOrDefault();

    public IReadOnlyList<DataCoreUser> GetAllUsers()
        => UsersInScope.AsReadOnly();
}
