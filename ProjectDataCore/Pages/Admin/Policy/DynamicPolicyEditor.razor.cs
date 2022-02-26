using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Policy;
using ProjectDataCore.Data.Structures.Roster;

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
    [Inject]
    public IAlertService AlertService { get; set; }
    [Inject]
    public IUserService UserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public List<DynamicAuthorizationPolicy> CurrentPolicies { get; set; } = new();
    public bool ButtonActive { get; set; } = true;
    /// <summary>
    /// Use of the buttons.
    /// </summary>
    /// <remarks>
    /// 0 - Start Edit
    /// <br />
    /// 1 - Select Parent
    /// </remarks>
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
            case 1:
                await AddParentAsync(policy);
                break;
        }
    }

    protected async Task StartEditAsync(DynamicAuthorizationPolicy policy)
    {
        ToEdit = policy;

        ButtonActive = false;

        // Load the selection values.
        var treeRes = await ModularRosterService.GetAllRosterTreesAsync();

        AllSlots.Clear();
        if(treeRes.GetResult(out var rosterTrees, out var err))
        {
            AllTrees = rosterTrees;
            foreach(var t in AllTrees)
            {
                TreeDisplayValues.Add(t.Name);
                foreach(var s in t.RosterPositions)
                {
                    AllSlots.Add(s);
                    SlotsDisplayValues.Add($"{t.Name} - {s.Name}");
                }
            }
        }
        else
        {
            AlertService.CreateErrorAlert(err);
            await StopEditAsync();
            return;
        }

        var displayRes = await ModularRosterService.GetAvalibleRosterDisplaysAsync();
        if(displayRes.GetResult(out var rosterDisplaySettings, out err))
        {
            AllDisplays = rosterDisplaySettings;
            foreach(var d in AllDisplays)
            {
                DisplayDisplayValues.Add(d.Name);
            }
        }
        else
        {
            AlertService.CreateErrorAlert(err);
            await StopEditAsync();
            return;
        }

        AllUsers = await UserService.GetAllUsersAsync();
        foreach (var u in AllUsers)
            UserDisplayValues.Add(u.UserName);

        // Ensure we are using the previously loaded objects for our selections.
        SelectedSlots.Clear();
        RosterSlot? slot;
        foreach (var s in ToEdit.AuthorizedSlots)
            if ((slot = AllSlots.FirstOrDefault(x => x.Key == s.Key) ?? null) is not null)
                SelectedSlots.Add(slot);

        SelectedTrees.Clear();
        RosterTree? tree;
        foreach (var s in ToEdit.AuthorizedTrees)
            if ((tree = AllTrees.FirstOrDefault(x => x.Key == s.Key) ?? null) is not null)
                SelectedTrees.Add(tree);

        SelectedDisplays.Clear();
        RosterDisplaySettings? disp;
        foreach (var s in ToEdit.AuthorizedDisplays)
            if ((disp = AllDisplays.FirstOrDefault(x => x.Key == s.Key) ?? null) is not null)
                SelectedDisplays.Add(disp);

        SelectedUsers.Clear();
        DataCoreUser? user;
        foreach (var s in ToEdit.AuthorizedUsers)
            if ((user = AllUsers.FirstOrDefault(x => x.Id == s.Id) ?? null) is not null)
                SelectedUsers.Add(user);

        // Load the parent values.
        ToEdit.Parents.Clear();

        await PolicyService.LoadParentsAsync(ToEdit);
        SelectedParents = ToEdit.Parents.ToList();

        StateHasChanged();
    }

    protected async Task StopEditAsync()
    {
        ButtonActive = true;
        ButtonUse = 0;
        ToEdit = null;
        await ReloadPolicyListAsync();
        StateHasChanged();
    }

    protected Task OpenParentSelectorAsync()
    {
        if(ToEdit is not null)
        {
            ToEdit.Parents.Clear();
            ToEdit.Parents = SelectedParents.ToList();

            ButtonUse = 1;
            ButtonActive = true;
            StateHasChanged();
        }

        return Task.CompletedTask;
    }

    protected async Task AddParentAsync(DynamicAuthorizationPolicy policy)
    {
        if (ToEdit is not null)
        {
            ToEdit.Parents.Add(policy);

            // TODO Check to make sure the parent can actually be added by initializing the policy.

            try
            {
                await ToEdit.InitalizePolicyAsync(ModularRosterService, DbContextFactory, test: true);

                SelectedParents.Add(policy);
            }
            catch (CircularPolicyException)
            {
                AlertService.CreateErrorAlert("A circular inheritance was detected and this parent was unable to be added.");
                ToEdit.Parents.Remove(policy);
            }
        }

        ButtonUse = 0;
        ButtonActive = false;
        StateHasChanged();
    }

    protected Task RemoveParentAsync(DynamicAuthorizationPolicy parent)
    {
        ToEdit?.Parents.Remove(parent);
        SelectedParents.Remove(parent);

        StateHasChanged();

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
        if (!string.IsNullOrWhiteSpace(NewPolicyName))
        {
            var res = await PolicyService.CreatePolicyAsync(new()
            {
                PolicyName = NewPolicyName
            });

            if (!res.GetResult(out var err))
                AlertService.CreateErrorAlert(err);
        }
        else
        {
            AlertService.CreateErrorAlert("The new policy must have a name.");
        }

        await StopEditAsync();
    }

    protected async Task SaveChangesAsync()
    {
        if (ToEdit is not null)
        {
            var res = await PolicyService.UpdatePolicyAsync(ToEdit.Key, (x) =>
            {
                x.PolicyName = ToEdit.PolicyName;
                x.AuthorizedSlots = SelectedSlots;
                x.AuthorizedTrees = SelectedTrees;
                x.AuthorizedDisplays = SelectedDisplays;
                x.AuthorizedUsers = SelectedUsers;

                x.Parents = SelectedParents;
            });

            if(!res.GetResult(out var err))
                AlertService.CreateErrorAlert(err);
        }

        await StopEditAsync();
    }

    #region Slots Selection
    public List<RosterSlot> SelectedSlots { get; set; } = new();
    public List<RosterSlot> AllSlots { get; set; } = new();
    public List<string> SlotsDisplayValues { get; set; } = new();

    protected void SlotsSelectionChanged()
    {
        StateHasChanged();
    }
    #endregion

    #region Tree Selection
    public List<RosterTree> SelectedTrees { get; set; } = new();
    public List<RosterTree> AllTrees { get; set; } = new();
    public List<string> TreeDisplayValues { get; set; } = new();

    protected void TreeSelectionChanged()
    {
        StateHasChanged();
    }
    #endregion

    #region Display Selection
    public List<RosterDisplaySettings> SelectedDisplays { get; set; } = new();
    public List<RosterDisplaySettings> AllDisplays { get; set; } = new();
    public List<string> DisplayDisplayValues { get; set; } = new();

    protected void DisplaySelectionChanged()
    {
        StateHasChanged();
    }
    #endregion

    #region User Selection
    public List<DataCoreUser> SelectedUsers { get; set; } = new();
    public List<DataCoreUser> AllUsers { get; set; } = new();
    public List<string> UserDisplayValues { get; set; } = new();

    protected void UserSelectionChanged()
    {
        StateHasChanged();
    }
    #endregion
}
