using FiveOhFirstDataCore.Data.Structures.Roster;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using FiveOhFirstDataCore.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class BattalionHome
    {
        public enum HailstormDisplayOption
        {
            Roster,
            PromotionBoard,
        }
#pragma warning disable CS8618 // Injections are non-nullable.
        /// <summary>
        /// The roster service.
        /// </summary>
        [Inject]
        public IRosterService RosterService { get; set; }
        /// <summary>
        /// the auth service.
        /// </summary>
        [Inject]
        public IAuthorizationService AuthService { get; set; }
        /// <summary>
        /// the uhh.. refreshy service.
        /// </summary>
        [Inject]
        public IAdvancedRefreshService AdvancedRefreshService { get; set; }
#pragma warning restore CS8618
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private HailstormDisplayOption Active { get; set; } = HailstormDisplayOption.Roster;
        private HailstormData Data { get; set; }
        private bool Loaded { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                Data = await RosterService.GetHailstormDataAsync();

                Loaded = true;

                StateHasChanged();

                AdvancedRefreshService.AddDataReloadListener("Promotion_Battalion", DataReloadRequested);
            }
        }

        private void OnTypeChange(HailstormDisplayOption option)
        {
            Active = option;
            StateHasChanged();
        }

        public async Task CallRefreshRequest()
        {
            await InvokeAsync(StateHasChanged);
        }

        public async Task DataReloadRequested()
        {
            Data = await RosterService.GetHailstormDataAsync();

            await CallRefreshRequest();
        }

        public void Dispose()
        {
            AdvancedRefreshService.RemoveDataReloadListener(DataReloadRequested);
        }
    }
}
