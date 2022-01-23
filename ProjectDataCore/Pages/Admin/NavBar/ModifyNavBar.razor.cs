using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Pages.Admin.NavBar
{
    public partial class ModifyNavBar : ComponentBase
    {
        [Inject] public INavModuleService NavModuleService { get; set; }

        List<NavModule> navModules = new();
        List<NavModule> allNavModules = new();

        private NavModule? Editing { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();


            navModules = await NavModuleService.GetAllModulesWithChildren();
            allNavModules = await NavModuleService.GetAllModules();
        }

        private void EditNavModule(NavModule navModule)
        {
            Editing = navModule;
            InvokeAsync(StateHasChanged);
        }

        private void NewNavModule(NavModule navModule)
        {
            Editing = new(navModule.Key);
            InvokeAsync(StateHasChanged);
        }

        private void NewParentNavModule()
        {
            Editing = new();
            InvokeAsync(StateHasChanged);
        }

        private async Task SaveNavModule()
        {
            await NavModuleService.CreateNavModuleAsync(Editing);
            navModules = await NavModuleService.GetAllModulesWithChildren();
            allNavModules = await NavModuleService.GetAllModules();
            Editing = null;
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateModule()
        {
            await NavModuleService.UpdateNavModuleAsync(Editing);
            await InvokeAsync(StateHasChanged);
        }

        private async Task HandleForm()
        {
            if (Editing.Key != Guid.Empty)
            {
                await UpdateModule();
            }
            else
                await SaveNavModule();

        }

        private async Task DeleteModule()
        {
            await NavModuleService.DeleteNavModule(Editing.Key);
            Editing = null;
            navModules = await NavModuleService.GetAllModulesWithChildren();
            await InvokeAsync(StateHasChanged);
        }
    }
}
