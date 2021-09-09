using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures.Updates;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace FiveOhFirstDataCore.Pages.Data
{
    partial class PublicTrooperProfile
    {
        [Parameter]
        public Trooper Trooper { get; set; }
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private Dictionary<CShop, List<ClaimUpdateData>> ShopPositions { get; set; } = new();

        public string[] ServiceStrings = new string[6];

        private TrooperFlag Flag { get; set; } = new();

        private bool LoadedAdditional { get; set; } = false;

        private TrooperDescription Description { get; set; } = new();
        private bool LoadedDescriptions { get; set; } = false;

        [CascadingParameter]
        private Trooper? LoggedIn { get; set; } = new();

        private List<Qualification> QualValues = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _state.OnPersisting += OnPersisting;
            _advRefresh.AddUserSpecificDataReloadListener(Trooper.Id, "ReloadPublicData", OnDataRefreshRequest);

            QualValues = (
                (Qualification[])Enum
                .GetValues(typeof(Qualification)))
                .AsQueryable<Qualification>()
                .Where(x => x != Qualification.None && (Trooper.Qualifications & x) == x)
                .ToList();

            var user = (await AuthStateTask).User;

            if(_state.TryTakeAsJson<Dictionary<CShop, List<ClaimUpdateData>>>("shop_positions", out var shopPositions))
            {
                ShopPositions = shopPositions ?? new();
            }
            else
            {
                ShopPositions = await _roster.GetCShopClaimsAsync(Trooper);
            }

            if(_state.TryTakeAsJson<string[]>("service_strings", out var serviceStrings))
            {
                ServiceStrings = serviceStrings ?? new string[6];
            }
            else
            {
                var now = DateTime.UtcNow.ToEst();
                ServiceStrings[0] = Trooper.LastPromotion.ToShortDateString();
                ServiceStrings[1] = Trooper.StartOfService.ToShortDateString();
                ServiceStrings[2] = now.Subtract(Trooper.LastPromotion).TotalDays.ToString("F0");
                ServiceStrings[3] = now.Subtract(Trooper.StartOfService).TotalDays.ToString("F0");
                ServiceStrings[4] = Trooper.LastBilletChange.ToShortDateString();
                ServiceStrings[5] = now.Subtract(Trooper.LastBilletChange).TotalDays.ToString("F0");
            }

            if (_state.TryTakeAsJson<bool>("loaded_additional", out var loadAdd))
            {
                LoadedAdditional = loadAdd;
            }
            else
            {
                if ((await _auth.AuthorizeAsync(user, "RequireNCO")).Succeeded)
                {
                    await LoadFlags();
                    LoadedAdditional = true;
                }
            }

            if (_state.TryTakeAsJson<bool>("loaded_descriptions", out var loadDesc))
            {
                LoadedDescriptions = loadDesc;
            }
            else
            {
                await LoadDescription();
                LoadedDescriptions = true;
            }

            _refresh.RefreshRequested += RefreshMe;
        }

        private void RefreshMe()
        {
            InvokeAsync(StateHasChanged);
        }

        private Task OnPersisting()
        {
            _state.PersistAsJson("shop_positions", ShopPositions);
            _state.PersistAsJson("service_strings", ServiceStrings);
            _state.PersistAsJson("loaded_additional", LoadedAdditional);
            _state.PersistAsJson("loaded_descriptions", LoadedDescriptions);
            return Task.CompletedTask;
        }

        #region Description
        private async Task LoadDescription()
        {
            await _roster.LoadDescriptionsAsync(Trooper);
            Trooper.Descriptions.Sort((x, y) => y.Order.CompareTo(x.Order));
        }

        private async Task SaveDescription(EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Description.Content)) return;

            var user = (await AuthStateTask).User;

            if ((await _auth.AuthorizeAsync(user, "RequireNCO")).Succeeded)
            {
                await _roster.SaveNewDescription(user, Trooper, Description);
                Description = new();
                Trooper.Descriptions.Sort((x, y) => y.Order.CompareTo(x.Order));
                _advRefresh.CallDataReloadRequest("ReloadPublicData", Trooper.Id);
            }
        }

        private TrooperDescription? CurrentDesc;

        private void OnDrag(TrooperDescription desc)
        {
            CurrentDesc = desc;
        }

        private async Task OnDrop(TrooperDescription desc)
        {
            if (CurrentDesc is not null && desc != CurrentDesc)
                await _roster.UpdateDescriptionOrderAsync(Trooper, CurrentDesc, desc.Order);
            CurrentDesc = null;

            _advRefresh.CallDataReloadRequest("ReloadPublicData", Trooper.Id);
        }

        private async Task DeleteDescription(TrooperDescription desc)
        {
            await _roster.DeleteDescriptionAsync(Trooper, desc);
            _advRefresh.CallDataReloadRequest("ReloadPublicData", Trooper.Id);
        }
        #endregion

        #region Flags
        private async Task LoadFlags()
        {
            await _roster.LoadPublicProfileDataAsync(Trooper);
            Trooper.Flags.Sort((x, y) => y.CreatedOn.CompareTo(x.CreatedOn));
        }

        private async Task SaveFlag(EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Flag.Contents)) return;

            var user = (await AuthStateTask).User;

            if ((await _auth.AuthorizeAsync(user, "RequireNCO")).Succeeded)
            {
                await _roster.SaveNewFlag(user, Trooper, Flag);
                Flag = new();
                Trooper.Flags.Sort((x, y) => y.CreatedOn.CompareTo(x.CreatedOn));
                _advRefresh.CallDataReloadRequest("ReloadPublicData", Trooper.Id);
            }
        }
        #endregion

        public async Task OnDataRefreshRequest()
        {
            Trooper = await _roster.GetTrooperFromIdAsync(Trooper.Id);
            await LoadFlags();
            await LoadDescription();

            await InvokeAsync(StateHasChanged);
        }

        void IDisposable.Dispose()
        {
            _state.OnPersisting -= OnPersisting;
            _refresh.RefreshRequested -= RefreshMe;
        }
    }
}
