using Microsoft.JSInterop;

using ProjectDataCore.Components.Framework.Page.Components;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.History;
using ProjectDataCore.Data.Services.Keybindings;
using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Keybindings;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components.Scope;

using System.Collections.Concurrent;
using System.Reflection;
using System.Web;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageEditComponent : IDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Inject is always non-null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
    [Inject]
    public IRoutingService RoutingService { get; set; }
    [Inject]
    public IEditHistoryService EditHistoryService { get; set; }
    [Inject]
    public IKeybindingService KeybindingService { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public delegate Task DraggableRefreshRequested(object sender);
    public event DraggableRefreshRequested? OnDragRefreshRequested;

    #region States
    public enum LeftMenu
    {
        PageSelection,
        ComponentSelection,
        SettingsSelection
    }

    public enum RightMenu
    {
        Empty,
        PageEditor,
        SettingsEditor
    }
    #endregion

    protected LeftMenu LeftMenuState { get; set; } = LeftMenu.PageSelection;
    protected RightMenu RightMenuState { get; set; } = RightMenu.Empty;

    #region Utility
    private List<(ComponentAttribute, string)>? _editorComponents = null;
    public IReadOnlyList<(ComponentAttribute, string)> EditorComponents 
    { 
        get
        {
            if (_editorComponents is null)
                _editorComponents = GetEditorComponents();

            return _editorComponents;
        }
    }

    private static List<(ComponentAttribute, string)> GetEditorComponents()
    {
        var curType = typeof(CustomComponentBase);
        var subclasses = Assembly.GetAssembly(curType)!.GetTypes()
            .Where(x => x.IsSubclassOf(curType))
            .ToList(x => (x.GetCustomAttributes(typeof(ComponentAttribute), true).FirstOrDefault() as ComponentAttribute, x.AssemblyQualifiedName))
            .Where(x => x.Item1 is not null);

        // We filtered out the null values already.
        return subclasses!.ToList()!;
    }
    #endregion

    #region Inital Setup
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await RefreshPageListAsync();
        }

        if (OpenConfigurationNodeFragment is not null)
        {
            if (HideConfigurationOptions &&
                !ShowConfigurationOptions)
			{
                OpenConfigurationNodeFragment = null;
                HideConfigurationOptions = false;
                StateHasChanged();
			}
            else if (!ShowConfigurationOptions)
            {
                ShowConfigurationOptions = true;
                StateHasChanged();
            }
        }

        if (RegisterDroppablesOnNextRender)
        {
            RegisterDroppablesOnNextRender = false;
            await RegisterDroppableAsync();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        KeybindingService.RegisterKeybindListener(Keybinding.Undo, OnUndoClickedAsync);
        KeybindingService.RegisterKeybindListener(Keybinding.Redo, OnRedoClickedAsync);
    }
    #endregion

    #region Page Control
    private bool SidebarVisible { get; set; } = true;

    private void ToggleSidebar()
    {
        SidebarVisible = !SidebarVisible;
        StateHasChanged();
    }
    #endregion

    #region Left Menu - Page Selection
    private List<CustomPageSettings> Pages { get; set; } = new();
    private HashSet<CustomPageSettings> FilteredPages { get; set; } = new();

    private class RouteOrder
    {
        public List<RouteOrder> Children { get; set; } = new();
        public CustomPageSettings? ThisNode { get; set; }

        public RouteOrder CreateOrReturnRouteParent(string route)
        {
            var parts = route.Split("/", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                var child = Children.FirstOrDefault(x => x.ThisNode?.Route.StartsWith(
                    $"{((ThisNode?.Route ?? "") == "/" ? "" : ThisNode?.Route ?? "")}/{parts[0]}") ?? false);

                if (child is null)
                {
                    child = new();
                    Children.Add(child);
                }

                if (parts.Length > 1)
                    return child.CreateOrReturnRouteParent(string.Join("/", parts[1..]));
                else
                    return child;
            }
            else
            {
                return this;
            }
        }
    }

    private List<(int, CustomPageSettings)> PageDisplayOrder { get; set; } = new();

    private bool DisplaySearch { get; set; } = false;
    private string NameSearch { get; set; } = "";
    private string RouteSearch { get; set; } = "";

    private CustomPageSettings? PageToEdit { get; set; }

    private bool DisplayNewPage { get; set; } = false;
    private string NewPageName { get; set; } = "";
    private string NewPageRoute { get; set; } = "";

    private async Task RefreshPageListAsync()
    {
        Pages = await PageEditService.GetAllPagesAsync();

        RouteOrder ordering = new();

        foreach (var p in Pages)
        {
            var order = ordering.CreateOrReturnRouteParent(p.Route);
            order.ThisNode = p;
        }

        // The integer counts route depth.
        Queue<(int, CustomPageSettings)> displayOrder = new();
        Stack<(int, RouteOrder)> dataStack = new();
        dataStack.Push((0, ordering));

        while (dataStack.TryPop(out var orderItem))
        {
            if (orderItem.Item2.ThisNode is not null)
                displayOrder.Enqueue((orderItem.Item1, orderItem.Item2.ThisNode));

            foreach (var item in orderItem.Item2.Children
                .AsEnumerable()
                .OrderBy(x => x.ThisNode?.Name))
            {
                dataStack.Push((orderItem.Item1 + 1, item));
            }
        }

        PageDisplayOrder.Clear();
        PageDisplayOrder.AddRange(displayOrder);

        UpdateSearch();

        StateHasChanged();
    }

    private void OnNameSearchChanged(string search)
    {
        NameSearch = search;

        UpdateSearch();
    }

    private void OnRouteSearchChanged(string search)
    {
        RouteSearch = search;

        UpdateSearch();
    }

    private void UpdateSearch()
    {
        FilteredPages = Pages.Where(x => x.Route.StartsWith(RouteSearch, StringComparison.OrdinalIgnoreCase))
            .Where(x => x.Name.StartsWith(NameSearch, StringComparison.OrdinalIgnoreCase))
            .ToHashSet();
    }

    private async Task StartPageEditAsync(CustomPageSettings settings)
    {
        // New editor? Rest the edit history.
        EditHistoryService.Reset();

        PageToEdit = settings;

        var loader = RoutingService.LoadPageSettingsAsync(settings);

        await foreach (var _ in loader)
            StateHasChanged();

        LeftMenuState = LeftMenu.ComponentSelection;
        RightMenuState = RightMenu.PageEditor;

        RegisterDroppablesOnNextRender = true;

        StateHasChanged();
    }

    private async Task AbortPageEditAsync()
    {
        PageToEdit = null;
        await RefreshPageListAsync();
        await DisposeDroppableAsync();

        LeftMenuState = LeftMenu.PageSelection;
        RightMenuState = RightMenu.Empty;
    }

    private async Task SavePageEditAsync()
    {
        // TODO: Save page edits code.

        PageToEdit = null;
        await RefreshPageListAsync();

        LeftMenuState = LeftMenu.PageSelection;
        RightMenuState = RightMenu.Empty;
    }

    private async Task CreateNewPageAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPageRoute))
        {
            AlertService.CreateErrorAlert("The page route must have a value", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPageName))
        {
            AlertService.CreateErrorAlert("The page name must have a value", true);
            return;
        }

        var oldRoute = NewPageRoute;
        NewPageRoute = HttpUtility.HtmlEncode(NewPageRoute);

        if (!oldRoute.Equals(NewPageRoute))
        {
            AlertService.CreateWarnAlert("The route was changed to be HTTP safe. Please verify this " +
                "is the route you would like and attempt to create a new page again.", true);
            return;
        }

        if (NewPageRoute.Contains('?')
            || NewPageRoute.Contains('&'))
        {
            AlertService.CreateErrorAlert("The route contains one or more of the following invalid characters: ? &");
            return;
        }

        if (!NewPageRoute.StartsWith("/"))
        {
            NewPageRoute = "/" + NewPageRoute;
        }

        var res = await PageEditService.CreateNewPageAsync(NewPageName, NewPageRoute);

        if (!res.GetResult(out var err))
        {
            AlertService.CreateErrorAlert(err);
        }
        else
        {
            NewPageName = "";
            NewPageRoute = "";
        }

        await RefreshPageListAsync();
    }
    #endregion

    #region Component Droppable Management
    public const string DROPPABLE_ID = "page-editor-global-draggable";
    public const string DROPPABLE_DROPZONE = "add-component";
    private DotNetObjectReference<PageEditComponent>? DotNetRef { get; set; }
    protected DotNetObjectReference<PageEditComponent> GetDotNetReference()
    {
        if (DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    private bool DraggingComponent { get; set; } = false;
    private bool RegisterDroppablesOnNextRender { get; set; } = false;

    [JSInvokable]
    public async Task OnDragChanged(bool started, string type)
    {
        DraggingComponent = started;
        await InvokeAsync(StateHasChanged);
    }

    private async Task RegisterDroppableAsync()
    {
        await JSRuntime.InvokeVoidAsync("DropInterop.init", DROPPABLE_ID, GetDotNetReference(), true, true, nameof(OnDragChanged), ".drag-component");
    }

    private async Task DisposeDroppableAsync()
    {
        await JSRuntime.InvokeVoidAsync("DropInterop.destroyDroppable", DROPPABLE_ID);
    }
    #endregion

    #region Left Menu - Page Settings
    public enum SettingState
    {
        Page,
        Node,
        Component
    }

    private ConcurrentDictionary<string, RenderFragment> ConfigurationNodes { get; set; } = new();
    private RenderFragment? OpenConfigurationNodeFragment { get; set; } = null;
    private bool ShowConfigurationOptions { get; set; } = false;
    private bool HideConfigurationOptions { get; set; } = false;
    private SettingState SettingMenuState { get; set; } = SettingState.Page;

    public delegate Task OnConfigureMenuClosedEventArgs(PageEditComponent sender);
    public event OnConfigureMenuClosedEventArgs? OnConfigureMenuClosed;

    public async Task OnConfigureNodePushed(string name, RenderFragment fragment, bool dispose)
    {
        if (dispose)
        {
            _ = ConfigurationNodes?.TryRemove(name, out _);
        }
        else
        {
            ConfigurationNodes[name] = fragment;
        }

        await InvokeAsync(StateHasChanged);
    }

    public async Task OpenPageSettingsAsync()
    {
        LeftMenuState = LeftMenu.SettingsSelection;
        RightMenuState = RightMenu.SettingsEditor;
        SettingMenuState = SettingState.Page;
        ConfigurationNodes.Clear();

        await DisposeDroppableAsync();
    }

    public async Task OpenNodeSettingsAsync()
    {
        LeftMenuState = LeftMenu.SettingsSelection;
        RightMenuState = RightMenu.SettingsEditor;
        SettingMenuState = SettingState.Node;
        ConfigurationNodes.Clear();

        await DisposeDroppableAsync();
    }

    public async Task OpenComponentSettingsAsync()
    {
        LeftMenuState = LeftMenu.SettingsSelection;
        RightMenuState = RightMenu.SettingsEditor;
        SettingMenuState = SettingState.Component;
        ConfigurationNodes.Clear();

        await DisposeDroppableAsync();
    }

    private async Task CloseSettingsMenuAsync()
    {
        LeftMenuState = LeftMenu.ComponentSelection;
        RightMenuState = RightMenu.PageEditor;

        StopUserScopeEdit();

        ConfigurationNodes.Clear();

        if (OnConfigureMenuClosed is not null)
            await OnConfigureMenuClosed.Invoke(this);

        RegisterDroppablesOnNextRender = true;
    }

    private void OpenConfigurationNode(string? key)
    {
        if (key is not null)
        {
            _ = ConfigurationNodes.TryGetValue(key, out var fragment);
            OpenConfigurationNodeFragment = fragment;
        }
		else
		{
            HideConfigurationOptions = true;
            ShowConfigurationOptions = false;
        }

        StateHasChanged();
    }

    public void CloseConfigurationNode()
        => OpenConfigurationNode(null);
    #endregion

    #region User Scopes
    private UserScope? UserScopeToEdit { get; set; } = null;
    private int NotListeningIndex { get; set; } = 0;
    private List<PageComponentSettingsBase> NotListeningTo { get; set; } = new();
    private int NotProvidingIndex { get; set; } = 0;
    private List<PageComponentSettingsBase> NotProvidingTo { get; set; } = new();

    private void AddUserScope()
    {
        if (PageToEdit is not null)
        {
            PageToEdit.UserScopes.Add(new());
            StateHasChanged();
        }
    }

    private void EditUserScope(UserScope scope)
    {
        UserScopeToEdit = scope;

        NotListeningTo.Clear();
        NotProvidingTo.Clear();
        NotListeningIndex = 0;
        NotProvidingIndex = 0;

        if (PageToEdit is not null)
        {
            var allChildren = PageToEdit.GetChildComponents();
            NotListeningTo = allChildren.Except(scope.GetOrderedProvidingComponents()).ToList();
            NotProvidingTo = allChildren.Except(scope.GetOrderedListeningComponents()).ToList();
        }

        StateHasChanged();
    }

    private void DeleteUserScope(UserScope scope)
    {
        if (PageToEdit is not null)
        {
            PageToEdit.UserScopes.Remove(scope);
            if (UserScopeToEdit == scope)
                StopUserScopeEdit();
            StateHasChanged();
        }
    }

    private void StopUserScopeEdit()
	{
        UserScopeToEdit = null;
	}

    private void ListenToComponent()
	{
        if (UserScopeToEdit is not null)
        {
            var component = NotListeningTo[NotListeningIndex];
            UserScopeToEdit.ScopeProviders.Add(new()
            {
                ProvidingComponent = component,
                Order = UserScopeToEdit.ScopeProviders.Count
            });
        }
	}

    private void StopListeningTo(UserScopeProviderContainer component)
	{
        if (UserScopeToEdit is not null)
		{
            UserScopeToEdit.ScopeProviders.Remove(component);
		}
	}

    private void ProvideForComponent()
	{
        if (UserScopeToEdit is not null)
        {
            var component = NotProvidingTo[NotProvidingIndex];
            UserScopeToEdit.ScopeListeners.Add(new()
			{
                ListeningComponent = component,
                Order = UserScopeToEdit.ScopeListeners.Count
			});
        }
    }

    private void StopProvidingTo(UserScopeListenerContainer component)
    {
        if (UserScopeToEdit is not null)
        {
            UserScopeToEdit.ScopeListeners.Remove(component);
        }
    }

    #endregion

    #region Undo/Redo
    private async Task OnUndoClickedAsync()
    {
        await EditHistoryService.UndoAsync();

        await InvokeAsync(StateHasChanged);
    }

    private async Task OnRedoClickedAsync()
    {
        await EditHistoryService.RedoAsync();

        await InvokeAsync(StateHasChanged);
    }
    #endregion

#nullable disable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                KeybindingService.RemoveKeybindListener(Keybinding.Undo, OnUndoClickedAsync);
                KeybindingService.RemoveKeybindListener(Keybinding.Redo, OnRedoClickedAsync);

                try
                {
                    DisposeDroppableAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // If this no longer exists, neither does the JS we want to
                    // remove.
                }
            }

            ConfigurationNodes = null;
            Pages = null;
            FilteredPages = null;
            PageDisplayOrder = null;
            PageToEdit = null;

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
#nullable enable
}
