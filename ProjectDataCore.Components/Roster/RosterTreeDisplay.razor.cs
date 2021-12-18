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

    [CascadingParameter(Name = "RosterEdit")]
    public bool Editing { get; set; } = false;
}
