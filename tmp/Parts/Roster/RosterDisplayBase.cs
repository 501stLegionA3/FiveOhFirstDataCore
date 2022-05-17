using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Roster;

public class RosterDisplayBase : CustomComponentBase
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IModularRosterService ModularRosterService { get; set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public RosterComponentSettings? ComponentData { get; set; }

    public RosterTree? ActiveTree { get; set; }
}
