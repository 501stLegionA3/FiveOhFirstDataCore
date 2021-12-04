using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using FiveOhFirstDataCore.Data.Structures.Roster;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class RazorHome
    {
        public enum RazorDisplayOption
        {
            Roster,
            RazorPromotionBoard,
            WardenPromotionBoard
        }

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private RazorDisplayOption Active { get; set; } = RazorDisplayOption.Roster;
        private RazorSquadronData? RazorData { get; set; }

        private WardenData? WardenData { get; set; }

        private bool Loaded { get; set; } = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var user = (await AuthStateTask).User;
                RazorData = await _roster.GetRazorDataAsync();
                WardenData = await _roster.GetWardenDataAsync();
                RazorData.Warden = WardenData;
                Loaded = true;
                StateHasChanged();
                _advRefresh.AddDataReloadListener("Promotion_Razor", RazorDataReloadRequested);
                _advRefresh.AddDataReloadListener("Promotion_Warden", WardenDataReloadRequested);
            }
        }

        private void OnTypeChange(RazorDisplayOption option)
        {
            Active = option;
            StateHasChanged();
        }

        public async Task CallRefreshRequest()
        {
            await InvokeAsync(StateHasChanged);
        }

        public async Task RazorDataReloadRequested()
        {
            RazorData = await _roster.GetRazorDataAsync();
            await CallRefreshRequest();
        }

        public async Task WardenDataReloadRequested()
        {
            WardenData = await _roster.GetWardenDataAsync();
            await CallRefreshRequest();
        }

        public void Dispose()
        {
            _advRefresh.RemoveDataReloadListener(RazorDataReloadRequested);
            _advRefresh.RemoveDataReloadListener(WardenDataReloadRequested);
        }
    }
}