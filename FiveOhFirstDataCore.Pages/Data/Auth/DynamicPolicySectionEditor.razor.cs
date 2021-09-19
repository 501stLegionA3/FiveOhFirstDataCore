using FiveOhFirstDataCore.Core.Structures.Policy;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class DynamicPolicySectionEditor
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IAlertService AlertService { get; set; }
    [Inject]
    public IWebsiteSettingsService WebsiteSettingsService {  get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public string SectionName { get; set; } = "default";

    private string SectionTitle { get; set; } = "n/a";
    private string SectionLoaction { get; set; } = "n/a";

    private PolicySection? Section { get; set; }
    private List<DynamicPolicy> DynamicPolicies { get; set; } = new();

    private bool ShowEditPage { get; set; } = false;
    private bool EditStarted { get; set; } = false;
    private DynamicPolicy PolicyToEdit { get; set; } = new();
    private string? PolicyToEditRaw { get; set; } = null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        try
        {
            var parts = SectionName.Split(".");
            SectionTitle = parts[1];
            SectionLoaction = parts[0];
        }
        catch
        {
            // Do nothing.
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var res = await WebsiteSettingsService.GetOrCreatePolicySectionAsync(SectionName);

            if(res.GetResult(out PolicySection sec, out List<string> err))
            {
                Section = sec;
                DynamicPolicies = await WebsiteSettingsService.GetDynamicPoliciesAsync();
            }
            else
            {
                AlertService.PostAlert(this, err);
            }

            StateHasChanged();
        }
    }

    private void CloseModal()
    {
        AlertService.PostModal(this, null);
    }

    private void OpenPolicyCreator()
    {
        ShowEditPage = !ShowEditPage;

        if(!ShowEditPage)
        {
            EditStarted = false;
            PolicyToEdit = null;
        }
    }

    private void StartEdit()
    {
        if (PolicyToEditRaw is null)
            PolicyToEdit = new();
        else
        {
            PolicyToEdit = DynamicPolicies.FirstOrDefault(x => x.PolicyName == PolicyToEditRaw) ?? new();
        }
        EditStarted = true;
    }

    private async Task SaveChanges()
    {
        if (Section is not null)
        {
            await WebsiteSettingsService.UpdatePolicySectionAsync(Section);
        }
        CloseModal();
    }
}
