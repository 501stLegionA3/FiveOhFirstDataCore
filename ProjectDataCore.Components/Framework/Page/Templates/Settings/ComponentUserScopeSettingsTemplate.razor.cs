using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Scope;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Templates.Settings;
public partial class ComponentUserScopeSettingsTemplate
{
#pragma warning disable CS8618 // Editor Required is never null.
    [Parameter, EditorRequired]
    public LayoutNode Node { get; set; }
    [Parameter, EditorRequired]
    public PageEditComponent EditComponent { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public bool AllowSendData { get; set; } = true;
    [Parameter]
    public bool AllowReceiveData { get; set; } = true;

    private PageComponentSettingsBase? _component;

    private int NotListeningIndex { get; set; } = 0;
    private List<UserScope> NotListeningTo { get; set; } = new();
    private int NotProvidingIndex { get; set; } = 0;
    private List<UserScope> NotProvidingTo { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        _component = Node.Component;

        RefreshScopes();
    }

    private void RefreshScopes()
    {
        if (_component is not null
            && Node.TryGetPageSettings(out var settings))
        {
            NotListeningTo = settings.UserScopes.Where(x => !x.ScopeListeners.Any(y => y.ListeningComponent == _component)).ToList();
            NotProvidingTo = settings.UserScopes.Where(x => !x.ScopeProviders.Any(y => y.ProvidingComponent == _component)).ToList();
        }
    }

    // Listen to provider.
    private void ListenToScope()
    {
        if (_component is not null)
        {
            var scope = NotListeningTo[NotListeningIndex];
            _component.ScopeProviders.Add(new()
            {
                ProvidingScope = scope,
                Order = _component.ScopeProviders.Count
            });
        }
    }

    private void StopListeningTo(UserScopeListenerContainer scopeContainer)
    {
        if (_component is not null)
        {
            _component.ScopeProviders.Remove(scopeContainer);
        }
    }

    // Provide for listener.
    private void ProvideForScope()
    {
        if (_component is not null)
        {
            var scope = NotProvidingTo[NotProvidingIndex];
            _component.ScopeListeners.Add(new()
            {
                ListeningScope = scope,
                Order = _component.ScopeListeners.Count
            });
        }
    }

    private void StopProvidingTo(UserScopeProviderContainer scopeContainer)
    {
        if (_component is not null)
        {
            _component.ScopeListeners.Remove(scopeContainer);
        }
    }
}
