using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Roster;
public partial class RosterTreeDisplay
{
#pragma warning disable CS8618 // Injections are not null.
    [Inject]
    public IModularRosterService ModularRosterService { get; set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public RosterTree? Tree { get; set; }
    [Parameter]
    public int Depth { get; set; } = 0;
    [Parameter]
    public RosterTree? Parent { get; set; }

    [CascadingParameter(Name = "RosterEdit")]
    public bool Editing { get; set; } = false;
    [CascadingParameter(Name = "RosterReloader")]
    public Func<bool, Task>? ReloadListener { get; set; }
    [CascadingParameter(Name = "OpenEdits")]
    public ConcurrentDictionary<Guid, bool> OpenEdits { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(Tree is not null)
        {
            Tree.ChildRosters = Tree.ChildRosters
                .OrderBy(x => x.Order.FirstOrDefault(y => y.ParentObjectId == Tree.Key)?.Order ?? 0)
                .ToList();

            if(OpenEdits.TryGetValue(Tree.Key, out var edit))
            {
                StartRosterSectionEdit();
            }
            else
            {
                EditingRosterSection = false;
            }
        }
    }

    #region Add Roster Section
    public int PendingAdd { get; set; } = -1;
    public string NewRosterSectionName { get; set; } = "";
    protected async Task AddNewRosterSection()
    {
        if(Tree is not null 
            && !string.IsNullOrWhiteSpace(NewRosterSectionName))
        {
            var res = await ModularRosterService.AddRosterTreeAsync(NewRosterSectionName, Tree.Key, PendingAdd);

            // TODO Error handling.

            SetPendingAdd(-1);

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }
    
    protected void SetPendingAdd(int pos)
    {
        PendingAdd = pos;
        NewRosterSectionName = "";
    }
    #endregion

    #region Edit Roster Section
    public bool EditingRosterSection { get; set; } = false;

    public string EditSectionName { get; set; } = "";
    public bool AddingDisplay { get; set; } = false;
    public string NewDisplayName { get; set; } = "";

    public bool ConfirmDelete { get; set; } = false;

    protected void ResetInnerDisplays()
    {
        AddingDisplay = false;
        ConfirmDelete = false;
    }

    protected void StartRosterSectionEdit()
    {
        ResetInnerDisplays();

        EditSectionName = Tree.Name;
        EditingRosterSection = true;
    }

    protected void CancelRosterSectionEdit()
    {
        ResetInnerDisplays();

        EditSectionName = "";
        EditingRosterSection = false;
        _ = OpenEdits.TryRemove(Tree.Key, out _);
    }

    protected async Task SaveRosterSectionEditAsync()
    {
        if (Tree is not null
            && !string.IsNullOrWhiteSpace(EditSectionName))
        {
            var res = await ModularRosterService.UpdateRosterTreeAsync(Tree.Key, x =>
                x.Name = EditSectionName);

            // TODO handle errors

            CancelRosterSectionEdit();

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }

    protected void ToggleAddDisplay()
    {
        if(!AddingDisplay)
            ResetInnerDisplays();

        AddingDisplay = !AddingDisplay;
        NewDisplayName = "";
    }

    protected async Task AddDisplayAsync()
    {
        if(Tree is not null
            && !string.IsNullOrWhiteSpace(NewDisplayName))
        {
            var res = await ModularRosterService.AddRosterDisplaySettingsAsync(NewDisplayName, Tree.Key);

            // TODO handle errors.

            CancelRosterSectionEdit();

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }

    protected void ToggleDeleteDisplay()
    {
        if (!ConfirmDelete)
            ResetInnerDisplays();

        ConfirmDelete = !ConfirmDelete;
    }

    protected async Task DeleteRosterTreeAsync()
    {
        if(Tree is not null)
        {
            var res = await ModularRosterService.RemoveRosterTreeAsync(Tree.Key);

            // TODO handle errors.

            if (ReloadListener is not null)
                await ReloadListener.Invoke(false);
        }
    }

    protected async Task OnRosterMoveUp()
    {
        if(Tree is not null
            && Parent is not null)
        {
            var newOrder = Tree.Order.FirstOrDefault(x => x.ParentObjectId == Parent.Key)?.Order ?? -1;
            // Move the value up by one.
            newOrder--;

            // If we go below zero this object an not move any higher.
            if (newOrder >= 0)
            {

                var res = await ModularRosterService.UpdateRosterTreeAsync(Tree.Key, x =>
                    x.Order = (Parent.Key, newOrder));

                // TODO handle errors

                CancelRosterSectionEdit();

                OpenEdits[Tree.Key] = true;

                if (ReloadListener is not null)
                    await ReloadListener.Invoke(true);
            }
        }
    }

    protected async Task OnRosterMoveDown()
    {
        if (Tree is not null && Parent is not null)
        {
            var newOrder = Tree.Order.FirstOrDefault(x => x.ParentObjectId == Parent.Key)?.Order ?? -1;
            // Move the value down by one.
            newOrder++;

            var res = await ModularRosterService.UpdateRosterTreeAsync(Tree.Key, x =>
                x.Order = (Parent.Key, newOrder));

            // TODO handle errors

            CancelRosterSectionEdit();

            OpenEdits[Tree.Key] = true;

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }
    #endregion
}
