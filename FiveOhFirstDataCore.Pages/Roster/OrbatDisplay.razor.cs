using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures.Roster;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Roster;

public partial class OrbatDisplay
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IRosterService Roster { get; set; }
    [Inject]
    public IAdvancedRefreshService AdvRefresh { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public OrbatData Data { get; set; }
    public Trooper? Adjutant { get; set; } = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            AdvRefresh.AddDataReloadListener("UserData", OnDataReloadRequests);
            Data = await Roster.GetOrbatDataAsync();
            Adjutant = await Roster.GetAcklayAdjutantAsync();
            StateHasChanged();
        }
    }

    public async Task CallRefreshRequest()
    {
        await InvokeAsync(StateHasChanged);
    }

    public async Task OnDataReloadRequests()
    {
        Data = await Roster.GetOrbatDataAsync();
        await CallRefreshRequest();
    }

    public void Dispose()
    {
        AdvRefresh.RemoveDataReloadListener(OnDataReloadRequests);
    }
}
