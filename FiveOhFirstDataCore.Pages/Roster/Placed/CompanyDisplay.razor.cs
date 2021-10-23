using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Roster;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Roster.Placed;

public partial class CompanyDisplay
{
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public CompanyData Company { get; set; }

    [Parameter]
    public bool Airborne { get; set; } = false;

    [Parameter]
    public Trooper? Adjutant { get; set; } = null;
}
