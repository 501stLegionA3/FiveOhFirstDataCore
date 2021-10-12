using FiveOhFirstDataCore.Data.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class TrooperDisplayPage
    {
        [Parameter]
        public string TrooperBirthNumber { get; set; } = "";

        private Trooper? Trooper { get; set; } = null;

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender || TrooperBirthNumber != Trooper?.BirthNumber.ToString())
            {
                var user = (await AuthStateTask).User;

                if (int.TryParse(TrooperBirthNumber, out int birthNumber))
                    Trooper = await _roster.GetTrooperFromBirthNumberAsync(birthNumber);
                else if (TrooperBirthNumber == "me")
                    Trooper = await _roster.GetTrooperFromClaimsPrincipalAsync(user);


                if (Trooper is null) Trooper = new();

                StateHasChanged();
            }
        }
    }
}
