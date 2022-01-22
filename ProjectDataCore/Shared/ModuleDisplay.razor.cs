using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Shared
{
    public partial class ModuleDisplay : ComponentBase
    {
        [Parameter]
        public NavModule Module { get; set; }

        [CascadingParameter]
        public string URI { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        private void Navigate(string href)
        {
            NavManager.NavigateTo(href, true);
        }
    }
}