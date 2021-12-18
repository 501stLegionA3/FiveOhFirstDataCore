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

    [CascadingParameter(Name = "RosterEdit")]
    public bool Editing { get; set; } = false;
}
