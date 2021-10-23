using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Roster;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Roster.Placed;

public partial class PlatoonStaff
{
    [Parameter]
    public PlatoonData Platoon { get; set; }
    [Parameter]
    public Trooper? Adjutant { get; set; } = null;
    [CascadingParameter(Name = "Airborne")]
    public bool Airborne { get; set; }
}
