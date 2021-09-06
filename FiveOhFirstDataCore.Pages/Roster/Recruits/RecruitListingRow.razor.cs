using FiveOhFirstDataCore.Core.Account;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages.Roster.Recruits
{
    public partial class RecruitListingRow
    {
        [Parameter]
        public Trooper Trooper { get; set; }
    }
}
