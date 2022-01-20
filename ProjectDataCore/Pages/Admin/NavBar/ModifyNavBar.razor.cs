using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Pages.Admin.NavBar
{
    public partial class ModifyNavBar : ComponentBase, IDisposable
    {
        [Inject] private INavModuleService _navModuleService { get; set; }

        List<NavModule> navModules = new List<NavModule>();
        List<NavModule> allNavModules = new List<NavModule>();

        NavModule? editing { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _navModuleService.OnDblClick += EditNavModule;
            _navModuleService.OnLeftClick += NewNavModule;

            if (true)
            {
                navModules = await _navModuleService.GetAllModulesWithChildren();
                allNavModules = await _navModuleService.GetAllModules();
            }
        }

        private void EditNavModule(object? sender, NavModule navModule)
        {
            editing = navModule;
            InvokeAsync(StateHasChanged);
        }

        private void NewNavModule(object? sender, NavModule navModule)
        {
            editing = new(navModule.Key);
            InvokeAsync(StateHasChanged);
        }

        private void NewParentNavModule()
        {
            editing = new();
            InvokeAsync(StateHasChanged);
        }

        private async Task SaveNavModule()
        {
            await _navModuleService.CreateNavModuleAsync(editing);
            navModules = await _navModuleService.GetAllModulesWithChildren();
            allNavModules = await _navModuleService.GetAllModules();
            editing = null;
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateModule()
        {
            await _navModuleService.UpdateNavModuleAsync(editing);
            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleForm()
        {
            if (editing.Key != Guid.Empty)
            {
                await UpdateModule();
            }
            else
                await SaveNavModule();

        }

        private async Task DeleteModule()
        {
            await _navModuleService.DeleteNavModule(editing.Key);
            editing = null;
            navModules = await _navModuleService.GetAllModulesWithChildren();
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            _navModuleService.OnDblClick -= EditNavModule;
            _navModuleService.OnLeftClick += NewNavModule;
        }
    }
}
