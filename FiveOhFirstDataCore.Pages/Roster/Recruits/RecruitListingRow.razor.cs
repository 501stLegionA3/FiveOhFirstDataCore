using FiveOhFirstDataCore.Core.Account;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Roster.Recruits
{
    public partial class RecruitListingRow
    {
        [Parameter]
        public Trooper Trooper { get; set; }
    }
}
