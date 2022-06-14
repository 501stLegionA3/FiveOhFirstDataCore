using ProjectDataCore.Components.Framework.Page;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Selectors.Settings;
public partial class AuthorizationPolicySelector
{
#pragma warning disable CS8618 // Required settings are never null. Injections are never null.
	[Inject]
	public IPolicyService PolicyService { get; set; }
	[Inject]
	public IAlertService AlertService { get; set; }

	[Parameter, EditorRequired]
	public LayoutNode Settings { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	[Parameter]
	public PageEditComponent? EditComponent { get; set; }

	private List<DynamicAuthorizationPolicy> Policies { get; set; } = new();
	private int PolicyIndex { get; set; } = -1;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		await ReloadPoliciesAsync();
	}

	private async Task ReloadPoliciesAsync()
	{
		var res = await PolicyService.GetAllPoliciesAsync();
		if (res.GetResult(out var policies, out var err))
		{
			Policies = policies;
			PolicyIndex = Policies.FindIndex(x => x.Key == Settings.AuthorizationPolicyKey);
		}
		else
		{
			AlertService.CreateErrorAlert(err);
			Policies.Clear();
			PolicyIndex = -1;
		}

		StateHasChanged();
	}

	private void OnSetPolicy()
	{
		Settings.AuthorizationPolicy = Policies.ElementAtOrDefault(PolicyIndex);
		StateHasChanged();
	}
}
