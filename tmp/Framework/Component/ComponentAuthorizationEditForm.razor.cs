using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Policy;

namespace ProjectDataCore.Components.Framework.Component;
public partial class ComponentAuthorizationEditForm
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
    [Inject]
    public IPolicyService PolicyService { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public PageComponentSettingsBase? ComponentData { get; set; }
    [Parameter]
    public Func<Task> OnSettingsClose { get; set; }
    [Parameter]
    public bool ShowSaveOperations { get; set; } = true;

    public bool RequireAuth { get; set; } = false;
    public DynamicAuthorizationPolicy? NewAuthorzationItem { get; set; }
    public int? AuthIndex { get; set; }
    public List<DynamicAuthorizationPolicy> AllAuthPolicies { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            var res = await PolicyService.GetAllPoliciesAsync();
            if(res.GetResult(out var policies, out var err))
            {
                AllAuthPolicies = policies;

                NewAuthorzationItem = ComponentData?.AuthorizationPolicy;

                if (NewAuthorzationItem is not null)
                {
                    for (int i = 0; i < AllAuthPolicies.Count; i++)
                    {
                        if (AllAuthPolicies[i].Key == NewAuthorzationItem.Key)
                            AuthIndex = i;
                    }
                }
            }
            else
            {
                AlertService.CreateErrorAlert(err);
            }

            RequireAuth = ComponentData?.RequireAuth ?? false;

            StateHasChanged();
        }
    }

    protected async Task SaveChangesAsync()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.UpdatePermissionsAsync(ComponentData.Key, RequireAuth, NewAuthorzationItem);

            if (!res.GetResult(out var err))
            {
                AlertService.CreateErrorAlert(err);
            }

            if (OnSettingsClose is not null)
                await OnSettingsClose.Invoke();
        }
    }

    protected async Task AbortChangesAsync()
    {
        if (OnSettingsClose is not null)
            await OnSettingsClose.Invoke();
    }

    protected void OnAuthIndexChanged(int? index)
    {
        if(index is not null)
        {
            NewAuthorzationItem = AllAuthPolicies[index.Value];
        }
        else
        {
            NewAuthorzationItem = null;
        }

        AuthIndex = index;
    }
}
