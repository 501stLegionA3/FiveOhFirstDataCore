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

    public List<UserScopeProviderContainer> ScopeProviders { get; set; } = new();
    public List<UserScopeListenerContainer> ScopeListeners { get; set; } = new();

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

    private List<PageComponentSettingsBase>? orderedProvidingComponents = null;
    public List<PageComponentSettingsBase> GetOrderedProvidingComponents()
    {
        bool recal = orderedProvidingComponents is null;

        if (!recal)
        {
            recal = orderedProvidingComponents!.Count != ScopeProviders.Count;
        }

        if (recal)
        {
            orderedProvidingComponents = ScopeProviders
                .OrderBy(x => x.Order)
                .ToList(x => x.ProvidingComponent);
        }

        // One final check.
        if (orderedProvidingComponents is null)
            orderedProvidingComponents = new();

        return orderedProvidingComponents;
    }

    private List<PageComponentSettingsBase>? orderedListeningComponents = null;
    public List<PageComponentSettingsBase> GetOrderedListeningComponents()
    {
        bool recal = orderedListeningComponents is null;

        if (!recal)
        {
            recal = orderedListeningComponents!.Count != ScopeListeners.Count;
        }

        if (recal)
        {
            orderedListeningComponents = ScopeListeners
                .OrderBy(x => x.Order)
                .ToList(x => x.ListeningComponent);
        }

        // One final check.
        if (orderedListeningComponents is null)
            orderedListeningComponents = new();

        return orderedListeningComponents;
    }

    public void AttachListener(PageComponentSettingsBase component)
    {
        UserScopeListenerContainer container = new()
        {
            ListeningComponent = component,
            ProvidingScope = this,
            Order = ScopeListeners.Count
        };

        this.ScopeListeners.Add(container);
        component.ScopeProviders.Add(container);
    }

    public void AttachProvider(PageComponentSettingsBase component)
    {
        UserScopeProviderContainer container = new()
        {
            ProvidingComponent = component,
            ListeningScope = this,
            Order = ScopeListeners.Count
        };

        this.ScopeProviders.Add(container);
        component.ScopeListeners.Add(container);
    }
}
