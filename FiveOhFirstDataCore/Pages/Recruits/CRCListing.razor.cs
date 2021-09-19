using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages.Recruits
{
    public partial class CRCListing
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IRosterService Roster { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private List<Trooper> Recruits { get; set; } = new();
        private List<Trooper> UTCCadets { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await BuildTrooperListsAsync();
                StateHasChanged();
            }
        }

        private async Task BuildTrooperListsAsync()
        {
            Recruits = await Roster.GetNewRecruitsAsync();
            UTCCadets = await Roster.GetCurrentUTCCadets();
        }
    }
}
