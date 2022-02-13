using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Policy;
public partial class DynamicPolicyEditor
{
#pragma warning disable CS8618 // Inject is not null.
    [Inject]
    public IPolicyService PolicyService { get; set; }
    [Inject]
    public IModularRosterService ModularRosterService { get; set; }
    [Inject]
    public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public List<DynamicAuthorizationPolicy> CurrentPolicies { get; set; } = new();
    public bool ButtonActive { get; set; } = true;
    public int ButtonUse { get; set; } = 0;

    public string NewPolicyName { get; set; } = "";

    public DynamicAuthorizationPolicy? ToEdit { get; set; } = null;
    public List<DynamicAuthorizationPolicy> SelectedParents { get; set; } = new();

    public string? Error { get; set; } = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await ReloadPolicyListAsync();
            StateHasChanged();
        }
    }

    protected async Task ReloadPolicyListAsync()
    {
        var res = await PolicyService.GetAllPoliciesAsync();
        if(res.GetResult(out var policies, out var err))
        {
            CurrentPolicies = policies.OrderBy(x => x.PolicyName).ToList();
        }
        else
        {
            // TODO handle errors.
        }
    }

    protected async Task PolicySelectedAsync(DynamicAuthorizationPolicy policy)
    {
        switch(ButtonUse)
        {
            case 0:
                await StartEditAsync(policy);
                break;
        }
    }

    protected Task StartEditAsync(DynamicAuthorizationPolicy policy)
    {
        ToEdit = policy;

        ButtonActive = false;
        StateHasChanged();

        return Task.CompletedTask;
    }

    protected async Task StopEditAsync()
    {
        ButtonActive = true;
        ButtonUse = 0;
        await ReloadPolicyListAsync();
    }

    protected async Task OpenParentSelectorAsync()
    {
        if(ToEdit is not null)
        {
            await PolicyService.LoadParentsAsync(ToEdit);

            SelectedParents = ToEdit.Parents;

            ButtonUse = 1;
            ButtonActive = true;
        }
    }

    protected Task AddParentAsync(DynamicAuthorizationPolicy policy)
    {
        if(ToEdit is not null)
            ToEdit.Parents.Add(policy);

        return Task.CompletedTask;
    }

    protected async Task SaveParentSelectorAsync()
    {
        if(ToEdit is not null)
        {
            Error = null;
            try
            {
                await ToEdit.InitalizePolicyAsync(ModularRosterService, DbContextFactory);
            }
            catch (CircularPolicyException ex)
            {
                // TODO move this to notification errors.
                Error = ex.Message;
            }
        }
    }

    protected async Task OnNewPolicyAsync()
    {

    }
}
