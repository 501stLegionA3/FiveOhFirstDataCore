using FiveOhFirstDataCore.Core.Structures.Policy;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class WebsitePolicyView
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IWebsiteSettingsService WebsiteSettings { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private List<DynamicPolicy> Policies { get; set; } = new();
    private List<bool> DisplayTree { get; set; } = new();
    private List<string> OrphanTree { get; set; } = new();
    private bool NewPolicy { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Policies = await WebsiteSettings.GetDynamicPoliciesAsync();
            Policies.ForEach(x =>
            {
                DisplayTree.Add(false);
                OrphanTree.Add("");
            });
            StateHasChanged();
        }
    }

    private async Task DeletePolicyAsync(DynamicPolicy p, string orphanToName)
    {
        var orphanTo = Policies.FirstOrDefault(x => x.PolicyName == orphanToName);

        var res = await WebsiteSettings.DeletePolicyAsync(p, orphanTo);

        if (res.GetResult(out var err))
        {
            AlertService.PostAlert(this, $"Section {p.PolicyName} deleted.");
            Policies.Remove(p);
            DisplayTree = new();
            Policies.ForEach(x =>
            {
                DisplayTree.Add(false);
                OrphanTree.Add("");
            });
        }
        else
        {
            AlertService.PostAlert(this, err);
        }
    }
}
