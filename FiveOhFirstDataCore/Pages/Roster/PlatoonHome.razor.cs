using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Structures;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using System;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Components.Base;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class PlatoonHome : IRefreshBase
    {
        public enum PlatoonDisplayOption
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
        public string PlatoonDesignation { get; set; } = "";

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private PlatoonDisplayOption Active { get; set; } = PlatoonDisplayOption.Roster;
        private PlatoonData? Data { get; set; } = null;
        private ZetaSectionData? ZetaData { get; set; } = null;
        private RazorSquadronData? RazorSquadronData { get; set; } = null;
        private WardenData? WardenData { get; set; } = null;
        private bool Loaded { get; set; } = false;
        private bool Airborne { get; set; } = false;
        private Slot Slot { get; set; }
        private int Number { get; set; } = 0;
        private string CompanyName { get; set; } = "Temp";

        private ZetaUTCSectionData? ZetaThree { get; set; } = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var user = (await AuthStateTask).User;

                IAssignable<Trooper>? dat = null;
                if (int.TryParse(PlatoonDesignation, out int id))
                {
                    Slot = (Slot)id;
                    dat = await RosterService.GetPlatoonDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
                }
                else if (PlatoonDesignation == "me")
                {
                    dat = await RosterService.GetPlatoonDataAsync(user);

                    var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                    if (t is not null)
                        Slot = (Slot)((int)t.Slot / 10 * 10);
                }

                if (dat is PlatoonData d)
                    Data = d;
                else if (dat is ZetaSectionData z)
                    ZetaData = z;
                else if (dat is ZetaUTCSectionData u)
                    ZetaThree = u;
                else if (dat is RazorSquadronData r)
                    RazorSquadronData = r;
                else if (dat is WardenData w)
                    WardenData = w;

                if (ZetaThree is null)
                {
                    if (Airborne)
                        AdvancedRefreshService.AddDataReloadListener("Promotion_Company", DataReloadRequested);
                    else
                        AdvancedRefreshService.AddDataReloadListener("Promotion_Platoon", DataReloadRequested);
                }

                if (Slot >= Slot.AcklayCompany && Slot < Slot.Mynock)
                    Airborne = true;
                CompanyName = Slot.AsFull().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

                Number = ((int)Slot / 10) % 10;

                Loaded = true;

                StateHasChanged();
            }
        }

        private void OnTypeChange(PlatoonDisplayOption option)
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
            if (int.TryParse(PlatoonDesignation, out int id))
            {
                Slot = (Slot)id;
                dat = await RosterService.GetPlatoonDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
            }
            else if (PlatoonDesignation == "me")
            {
                dat = await RosterService.GetPlatoonDataAsync(user);

                var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                if (t is not null)
                    Slot = (Slot)((int)t.Slot / 10 * 10);
            }

            if (dat is PlatoonData d)
                Data = d;
            else if (dat is ZetaSectionData z)
                ZetaData = z;
            else if (dat is RazorSquadronData r)
                RazorSquadronData = r;
            else if (dat is WardenData w)
                WardenData = w; 

            await CallRefreshRequest();
        }

        public void Dispose()
        {
            AdvancedRefreshService.RemoveDataReloadListener(DataReloadRequested);
        }
    }
}
