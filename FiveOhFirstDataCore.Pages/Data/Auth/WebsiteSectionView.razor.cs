using FiveOhFirstDataCore.Core.Structures.Policy;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class WebsiteSectionView
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IWebsiteSettingsService WebsiteSettings { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private List<PolicySection> Sections { get; set; } = new();
    private List<bool> DisplayTree { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Sections = await WebsiteSettings.GetAllPolicySectionsAsync();
            Sections.ForEach(x => DisplayTree.Add(false));
            StateHasChanged();
        }
    }

    private async Task DeleteSectionAsync(PolicySection s)
	{
        var res = await WebsiteSettings.DeletePolicySectionAsync(s);

        if(res.GetResult(out var err))
		{
            AlertService.PostAlert(this, $"Section {s.SectionName} deleted.");
            Sections.Remove(s);
            DisplayTree = new();
            Sections.ForEach(x => DisplayTree.Add(false));
        }
        else
		{
            AlertService.PostAlert(this, err);
		}
    }
}
