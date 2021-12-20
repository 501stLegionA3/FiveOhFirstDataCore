using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Roster;
public partial class RosterSlotsDisplay
{
#pragma warning disable CS8618 // Injections are not null.
    [Inject]
    public IModularRosterService ModularRosterService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public List<RosterSlot> Slots { get; set; } = new();
    [Parameter]
    public int Depth { get; set; }
    [Parameter]
    public RosterTree Parent { get; set; }

    [CascadingParameter(Name = "RosterEdit")]
    public bool Editing { get; set; } = false;
    [CascadingParameter(Name = "RosterReloader")]
    public Func<bool, Task>? ReloadListener { get; set; }

    public string NewRosterSlotName { get; set; } = "";
    public RosterSlot? SlotToEdit { get; set; }
    public string EditSlotName { get; set; } = "";

    public RosterSlot? ConfirmDelete { get; set; }

    protected async Task OnSlotMoveUp(RosterSlot slot)
    {
        if(Parent is not null)
        {
            var newOrder = slot.Order.Order - 1;
            if (newOrder >= 0)
            {

                var res = await ModularRosterService.UpdateRosterSlotAsync(slot.Key,
                    x => x.Order = newOrder);

                // TODO handle errors

                if (ReloadListener is not null)
                    await ReloadListener.Invoke(true);
            }
        }
    }

    protected async Task OnSlotMoveDown(RosterSlot slot)
    {
        if(Parent is not null)
        {
            var newOrder = slot.Order.Order + 1;
            if (newOrder < Slots.Count)
            {
                var res = await ModularRosterService.UpdateRosterSlotAsync(slot.Key,
                    x => x.Order = newOrder);

                // TODO handle errors

                if (ReloadListener is not null)
                    await ReloadListener.Invoke(true);
            }
        }
    }

    protected async Task OnAddRosterSlot()
    {
        if(Parent is not null
            && !string.IsNullOrWhiteSpace(NewRosterSlotName))
        {
            var res = await ModularRosterService.AddRosterSlotAsync(NewRosterSlotName, Parent.Key, Slots.Count);

            // TODO handle errors

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }

    protected void OnEditSlot(RosterSlot? slot)
    {
        SlotToEdit = slot;
        EditSlotName = slot?.Name ?? "";
    }

    protected async Task OnSaveSlotEdit()
    {
        if(SlotToEdit is not null
            && !string.IsNullOrWhiteSpace(EditSlotName))
        {
            var res = await ModularRosterService.UpdateRosterSlotAsync(SlotToEdit.Key,
                x => x.Name = EditSlotName);

            // TODO handle errors.

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);

            OnEditSlot(null);
        }
    }

    protected void OnStartDelete(RosterSlot? slot)
    {
        ConfirmDelete = slot;
    }

    protected async Task OnDeleteSlot()
    {
        if(ConfirmDelete is not null)
        {
            var res = await ModularRosterService.RemoveRosterSlotAsync(ConfirmDelete.Key);

            // TODO handle errors

            if (ReloadListener is not null)
                await ReloadListener.Invoke(true);
        }
    }
}
