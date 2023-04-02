using FiveOhFirstDataCore.Data.Components.Base;
using FiveOhFirstDataCore.Data.Structures.Roster;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Roster.Placed
{
    public partial class RazorSquadronDisplay : RosterModel
    {
        [Parameter]
        [EditorRequired]
        public RazorSquadronData Squadron { get; set; }
        [Parameter]
        [EditorRequired]
        public string Name { get; set; }
    }
}