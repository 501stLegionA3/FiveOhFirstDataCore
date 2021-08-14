using FiveOhFirstDataCore.Core.Account;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperChangeRequest
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        public TrooperChangeRequestData Model { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
