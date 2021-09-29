using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Updates;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FiveOhFirstDataCore.Components.Data
{
    partial class PublicTrooperProfile
    {
        [Parameter]
        public Trooper Trooper { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        public bool FirstRender { get; set; } = true;

        public Trooper? Superior { get; set; }

        private Dictionary<CShop, List<ClaimUpdateData>> ShopPositions { get; set; } = new();

        public string[] ServiceStrings = new string[6];

        private TrooperFlag Flag { get; set; } = new();

        private bool IsNCO { get; set;} = false;

        private TrooperDescription Description { get; set; } = new();

        [CascadingParameter]
        private Trooper? LoggedIn { get; set; } = new();

        private List<Qualification> QualValues = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _advRefresh.AddUserSpecificDataReloadListener(Trooper.Id, "ReloadPublicData", OnDataRefreshRequest);

                QualValues = ((Qualification[])Enum.GetValues(typeof(Qualification))).AsQueryable<Qualification>()
                .Where(x => x != Qualification.None && (Trooper.Qualifications & x) == x).ToList();

                var user = (await AuthStateTask).User;

                ShopPositions = await _roster.GetCShopClaimsAsync(Trooper);

                var now = DateTime.UtcNow.ToEst();
                ServiceStrings[0] = Trooper.StartOfService.ToShortDateString();
                ServiceStrings[1] = now.Subtract(Trooper.StartOfService).TotalDays.ToString("F0");
                ServiceStrings[2] = Trooper.LastPromotion.ToShortDateString();
                ServiceStrings[3] = now.Subtract(Trooper.LastPromotion).TotalDays.ToString("F0");
                ServiceStrings[4] = Trooper.LastBilletChange.ToShortDateString();
                ServiceStrings[5] = now.Subtract(Trooper.LastBilletChange).TotalDays.ToString("F0");

                await LoadFlags();

                await LoadDescription();

                Superior = await _roster.GetDirectSuperior(Trooper);

                IsNCO = (await _auth.AuthorizeAsync(user, "RequireNCO")).Succeeded;

                _refresh.RefreshRequested += RefreshMe;
                RefreshMe();
            }


        }

        private void RefreshMe()
        {
            InvokeAsync(StateHasChanged);
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

        public void ChangeTrooper(int t)
        {
            Nav.NavigateTo($"/trooper/{t}", true);
        }

        public async Task OnDataRefreshRequest()
        {
            Trooper = await _roster.GetTrooperFromIdAsync(Trooper.Id);
            await LoadFlags();
            await LoadDescription();

            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Checks if the person logged in is the same as the person currently displayed.
        /// </summary>
        /// <returns>True if they are the same.</returns>
        public bool IsMe()
        {
            if (LoggedIn is null) return false;
            if (LoggedIn.Id == Trooper.Id) return true;
            return false;
        }


        void IDisposable.Dispose()
        {
            _refresh.RefreshRequested -= RefreshMe;
            _advRefresh.RemoveDataReloadListener(OnDataRefreshRequest);
        }
    }
}
