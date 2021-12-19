using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Roster;
public partial class RosterTreeDisplay
{
    [Parameter]
    public RosterTree Tree { get; set; }
    [Parameter]
    public int Depth { get; set; } = 0;

    [CascadingParameter(Name = "RosterEdit")]
    public bool Editing { get; set; } = false;
    [CascadingParameter(Name = "RosterReloader")]
    public Func<Task>? ReloadListener { get; set; }

    public int PendingAdd { get; set; } = -1;
    public string NewRosterSectionName { get; set; }
}
