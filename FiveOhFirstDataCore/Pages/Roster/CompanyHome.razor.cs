using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Structures;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using FiveOhFirstDataCore.Data.Components.Base;
using FiveOhFirstDataCore.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class CompanyHome : IRefreshBase
    {
        public enum CompanyDisplayOption
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

        [Parameter]
        public string CompanyDesignation { get; set; } = "";

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private CompanyDisplayOption Active { get; set; } = CompanyDisplayOption.Roster;
        private CompanyData? Data { get; set; } = null;
        private ZetaCompanyData? ZetaData { get; set; } = null;
        private RazorWingData? RazorWingData { get; set; } = null;
        private bool Loaded { get; set; } = false;
        private Slot Slot { get; set; }
        private bool Airborne { get; set; } = false;
        private string CompanyName { get; set; } = "Temp";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var user = (await AuthStateTask).User;

                IAssignable<Trooper>? dat = null;
                if (int.TryParse(CompanyDesignation, out int id))
                {
                    Slot = (Slot)id;
                    dat = await RosterService.GetCompanyDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
                }
                else if (CompanyDesignation == "me")
                {
                    dat = await RosterService.GetCompanyDataAsync(user);

                    var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                    if (t is not null)
                        Slot = (Slot)((int)t.Slot / 100 * 100);
                }

                if (dat is CompanyData d)
                    Data = d;
                else if (dat is ZetaCompanyData z)
                    ZetaData = z;
                else if (dat is RazorWingData r)
                    RazorWingData = r;

                Airborne = Slot >= Slot.AcklayCompany && Slot < Slot.Mynock;
                CompanyName = Slot.AsFull().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                Loaded = true;

                StateHasChanged();

                AdvancedRefreshService.AddDataReloadListener("Promotion_Company", DataReloadRequested);
            }
        }

        private void OnTypeChange(CompanyDisplayOption option)
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
            var user = (await AuthStateTask).User;

            IAssignable<Trooper>? dat = null;
            if (int.TryParse(CompanyDesignation, out int id))
            {
                Slot = (Slot)id;
                dat = await RosterService.GetCompanyDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
            }
            else if (CompanyDesignation == "me")
            {
                dat = await RosterService.GetCompanyDataAsync(user);

                var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                if (t is not null)
                    Slot = (Slot)((int)t.Slot / 100 * 100);
            }

            if (dat is CompanyData d)
                Data = d;
            else if (dat is ZetaCompanyData z)
                ZetaData = z;
            else if (dat is RazorWingData r)
                RazorWingData = r;

            await CallRefreshRequest();
        }

        public void Dispose()
        {
            AdvancedRefreshService.RemoveDataReloadListener(DataReloadRequested);
        }
    }
}
