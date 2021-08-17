using FiveOhFirstDataCore.Core.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Personnel.Roster
{
    public partial class EparApproval
    {
        protected List<TrooperChangeRequestData> ChangeRequests { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);


        }
    }
}
