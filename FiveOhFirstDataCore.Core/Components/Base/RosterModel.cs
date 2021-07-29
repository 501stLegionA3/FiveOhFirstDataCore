using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components.Base
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
