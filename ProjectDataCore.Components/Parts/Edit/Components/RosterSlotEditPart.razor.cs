using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit.Components;

[EditableComponent(Name = "Roster Slot Editor")]
public partial class RosterSlotEditPart : EditBase
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IModularRosterService RosterService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public List<RosterSlot> RegisteredSlots { get; set; } = new();

    public List<RosterDisplaySettings> DisplaySettings { get; set; } = new();
    public int SelectedDisplay { get; set; }

    public List<RosterTree> DisplayTrees { get; set; } = new();
    public int SelectedTree { get; set; }

    public List<RosterSlot> RosterSlots { get; set; } = new();
    public int SelectedSlot { get; set; }

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

    }

    protected async Task ReloadRosterDisplaysAsync()
    {
        if (ComponentData is not null)
        {
            await RosterService.LoadEditableDisplaysAsync(ComponentData);

            if(ComponentData.EditableDisplays.Count <= 0)
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
}
