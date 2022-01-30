using ProjectDataCore.Data.Structures.Model.User;
using ProjectDataCore.Data.Structures.Page;

namespace ProjectDataCore.Components.Parts.Edit.Components;

[EditableComponent(Name = "Roster Slot Editor")]
public partial class RosterSlotEditPart : EditBase, ISubmittable
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IModularRosterService RosterService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    #region Editing
    public List<RosterDisplaySettings> AllDisplaySettings { get; set; } = new();
    public int AllDisplaySettingsSelector { get; set; }
    #endregion

    public List<RosterSlot> RegisteredSlots { get; set; } = new();

    public List<RosterDisplaySettings> DisplaySettings { get; set; } = new();
    public int SelectedDisplay { get; set; } = -1;

    public List<RosterTree> DisplayTrees { get; set; } = new();
    public int SelectedTree { get; set; } = -1;

    public List<RosterSlot> RosterSlots { get; set; } = new();
    public int SelectedSlot { get; set; } = -1;

    #region Editing
    protected async Task OnStartComponentEditAsync()
	{
        if (ComponentData is not null)
        {
            base.StartEdit();

            EditModel!.AllowedDisplays = new();
            var res = await RosterService.LoadEditableDisplaysAsync(ComponentData);

            if(res.GetResult(out var err))
			{
                EditModel.AllowedDisplays.AddRange(ComponentData.EditableDisplays); 
                
                var allRes = await RosterService.GetAvalibleRosterDisplaysAsync();

                if (allRes.GetResult(out var displays, out var errTwo))
                {
                    AllDisplaySettings = displays;
                }
                else
                {
                    // TODO handle errors
                    CancelEdit();
                }
            }
			else
			{
                // TODO handle errors
                CancelEdit();
			}

            StateHasChanged();
        }
	}

    protected void OnAddAllowedRosterDisplay()
	{
        if(EditModel?.AllowedDisplays is not null)
            EditModel.AllowedDisplays.Add(AllDisplaySettings[AllDisplaySettingsSelector]);

        StateHasChanged();
	}

    protected void OnRemoveAllowedRosterDisplay(RosterDisplaySettings settings)
	{
        if (EditModel?.AllowedDisplays is not null)
            EditModel.AllowedDisplays.Remove(settings);

        StateHasChanged();
    }
	#endregion

	#region Submission
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

        if (ParentForm is not null)
        {
            ParentForm.AddSubmitListener(ScopeIndex, OnSubmitAsync);
        }
    }

	public Task OnSubmitAsync(DataCoreUserEditModel model)
    {
        if (ComponentData is not null)
        {
            model.Slots = RegisteredSlots;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if(ParentForm is not null)
		{
            ParentForm.RemoveSubmitListener(ScopeIndex, OnSubmitAsync);
		}
    }
    #endregion

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await ReloadRosterDisplaysAsync();
        }
    }
    
    protected async Task LoadSlotsAsync()
    {
        if (ScopedUsers is not null && ScopedUsers[ScopeIndex] is not null)
        {
            var res = await RosterService.LoadExistingSlotsAsync(ScopedUsers[ScopeIndex]);

            if(!res.GetResult(out var err))
            {
                // TOOD handle errors
            }
            else
            {
                RegisteredSlots.AddRange(ScopedUsers[ScopeIndex].RosterSlots);
            }
        }
    }

    protected async Task ReloadRosterDisplaysAsync()
    {
        if (ComponentData is not null)
        {
            await RosterService.LoadEditableDisplaysAsync(ComponentData);

            if(ComponentData.EditableDisplays.Count > 0)
            {
                DisplaySettings = ComponentData.EditableDisplays;
            }
            else
            {
                var res = await RosterService.GetAvalibleRosterDisplaysAsync();

                if (res.GetResult(out var displays, out var err))
                {
                    DisplaySettings = displays;
                }
                else
                {
                    // TODO handle errors
                }
            }

            await LoadSlotsAsync();

            StateHasChanged();
        }
    }

    protected async Task OnSelectedDisplayChangedAsync(int newIndex)
    {
        SelectedDisplay = newIndex;
        var res = await RosterService.GetRosterTreeForSettingsAsync(DisplaySettings[SelectedDisplay].Key);

        if(res.GetResult(out var tree, out var err))
        {
            DisplayTrees.Clear();
            DisplayTrees.Add(tree);

            // Load in the roster
            await foreach (var i in RosterService.LoadFullRosterTreeAsync(tree))
                continue;

            Stack<RosterTree> trees = new();
            trees.Push(tree);
            
            // List all tree parts
            while (trees.TryPop(out var t))
            {
                DisplayTrees.Add(t);
                foreach (var child in t.ChildRosters)
                    trees.Push(child);
            }


            // Clear old data.
            SelectedTree = -1;
            RosterSlots.Clear();
            SelectedSlot = -1;
        }
        else
        {
            // TODO show errors
        }

        StateHasChanged();
    }

    protected void OnSelectedTreeChanged(int newIndex)
    {
        SelectedTree = newIndex;

        RosterSlots.Clear();

        foreach(var s in DisplayTrees[SelectedTree].RosterPositions)
            RosterSlots.Add(s);

        // Clear old data.
        SelectedSlot = -1;

        StateHasChanged();
    }

    protected void OnSelectedSlotChanged(int newIndex)
    {
        SelectedSlot = newIndex;

        StateHasChanged();
    }

    protected void OnAddRosterSlot()
    {
        RegisteredSlots.Add(RosterSlots[SelectedSlot]);

        ClearSelection();

        StateHasChanged();
    }

    protected void OnRemoveRosterSlot(RosterSlot slot)
    {
        RegisteredSlots.Remove(slot);

        StateHasChanged();
    }

    protected void ClearSelection()
    {
        SelectedDisplay = -1;

        DisplayTrees.Clear();
        SelectedTree = -1;

        RosterSlots.Clear();
        SelectedSlot = -1;
    }
}
