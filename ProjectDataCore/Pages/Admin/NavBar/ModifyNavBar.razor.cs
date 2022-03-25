using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Nav;
using ProjectDataCore.Data.Structures.Policy;

namespace ProjectDataCore.Pages.Admin.NavBar
{
    public partial class ModifyNavBar : ComponentBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        public INavModuleService NavModuleService { get; set; }
        [Inject]
        public IPageEditService PageEditService { get; set; }
        [Inject]
        public IPolicyService PolicyService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        List<NavModule> navModules = new();
        List<NavModule> allNavModules = new();
        List<CustomPageSettings> allPages = new();
        List<DynamicAuthorizationPolicy>? allPolicies = new();

        bool LinkToPage = false;

        private NavModule? Editing { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();


            navModules = await NavModuleService.GetAllModulesWithChildren();
            allNavModules = await NavModuleService.GetAllModules();
            allPages = await PageEditService.GetAllPagesAsync();
            _ = (await PolicyService.GetAllPoliciesAsync()).GetResult(out allPolicies, out _);
        }

        private void EditNavModule(NavModule navModule)
        {
            Editing = navModule;
            LinkToPage = navModule.PageId is not null;
            InvokeAsync(StateHasChanged);
        }

        private void NewNavModule(NavModule navModule)
        {
            Editing = new(navModule.Key);
            LinkToPage = false;
            InvokeAsync(StateHasChanged);
        }

        private void NewParentNavModule()
        {
            Editing = new();
            LinkToPage = false;
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

        private void PageSelect(Guid? id)
        {
            Editing.AuthKey = allPages.Where(x => x.Key == id).FirstOrDefault()?.Layout?.AuthorizationPolicyKey;
            Editing.PageId = id;
        }
    }
}
