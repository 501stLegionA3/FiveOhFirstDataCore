using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Components.Roster.Recruits
{
    public partial class RecruitListingRow
    {
        [Parameter]
        public Trooper Trooper { get; set; }
    }
}
