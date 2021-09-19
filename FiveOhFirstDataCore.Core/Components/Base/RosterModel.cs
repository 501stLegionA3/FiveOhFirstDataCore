using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Components.Base
{
    public class RosterModel : ComponentBase
    {
        [Inject]
        private NavigationManager _nav { get; set; }

        public void TrooperNav(int? id)
        {
            if (id is not null)
                _nav.NavigateTo($"/trooper/{id.Value}");
        }
    }
}
