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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public bool AllowSendData { get; set; } = true;
    [Parameter]
    public bool AllowReceiveData { get; set; } = true;
    [CascadingParameter(Name = "PageEditComponent")]
    public PageEditComponent? EditComponent { get; set; }

    private PageComponentSettingsBase? _component;

    private int ScopeIndex { get; set; } = 0;
    private List<UserScope> AvalibleScopes { get; set; } = new();

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
            AvalibleScopes = settings.UserScopes
                .Where(x => !x.ScopeListeners.Any(y => y.ListeningComponent == _component))
                .Where(x => !x.ScopeProviders.Any(y => y.ProvidingComponent == _component))
                .ToList();
        }
    }

    // Listen to provider.
    private void ListenToScope()
    {
        if (_component is not null
            && ScopeIndex >= 0
            && ScopeIndex < AvalibleScopes.Count)
        {
            var scope = AvalibleScopes[ScopeIndex];
            scope.AttachListener(_component);

            RefreshScopes();
        }
    }

    private void StopListeningTo(UserScopeListenerContainer scopeContainer)
    {
        if (_component is not null)
        {
            scopeContainer.Deatch();

            RefreshScopes();
        }
    }

    // Provide for listener.
    private void ProvideForScope()
    {
        if (_component is not null
            && ScopeIndex >= 0
            && ScopeIndex < AvalibleScopes.Count)
        {
            var scope = AvalibleScopes[ScopeIndex];
            scope.AttachProvider(_component);

            RefreshScopes();
        }
    }

    private void StopProvidingTo(UserScopeProviderContainer scopeContainer)
    {
        if (_component is not null)
        {
            scopeContainer.Deatch();

            RefreshScopes();
        }
    }
}
