using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Roster;
public partial class RosterSlotsDisplay
{
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
}
