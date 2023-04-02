using AngleSharp.Io;
using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Structures;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using FiveOhFirstDataCore.Data.Components.Base;
using FiveOhFirstDataCore.Data.Services;
using Microsoft.AspNetCore.Authorization;

namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class SquadHome : IRefreshBase
    {
        public enum SquadDisplayOption
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
        public IAdvancedRefreshService AdvRefreshService { get; set; }
#pragma warning restore CS8618

        [Parameter]
        public string SquadDesignation { get; set; } = "";

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        private SquadDisplayOption Active { get; set; } = SquadDisplayOption.Roster;
        private SquadData? Data { get; set; } = null;
        private ZetaSquadData? ZetaData { get; set; } = null;
        private RazorFlightData? FlightData { get; set; } = null;
        private WardenFlightData? WardenData { get; set; } = null;
        private bool Loaded { get; set; } = false;
        private Slot Slot { get; set; }
        private bool Airborne { get; set; } = false;
        private int Number { get; set; } = 0;

        private ZetaUTCSquadData? ZetaThree { get; set; } = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var user = (await AuthStateTask).User;

                IAssignable<Trooper>? dat = null;
                if (int.TryParse(SquadDesignation, out int id))
                {
                    Slot = (Slot)id;
                    dat = await RosterService.GetSquadDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
                }
                else if (SquadDesignation == "me")
                {
                    dat = await RosterService.GetSquadDataAsync(user);

                    var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                    if (t is not null)
                        Slot = t.Slot;
                }

                if (dat is SquadData d)
                    Data = d;
                else if (dat is ZetaSquadData z)
                    ZetaData = z;
                else if (dat is ZetaUTCSquadData u)
                    ZetaThree = u;
                else if (dat is RazorFlightData r)
                    FlightData = r;
                else if (dat is WardenFlightData w)
                    WardenData = w;

                if (ZetaThree is null)
                {
                    AdvRefreshService.AddDataReloadListener("Promotion_Squad", DataReloadRequested);
                }

                Airborne = Slot >= Slot.AcklayCompany && Slot < Slot.Mynock;
                Number = (int)Slot % 10;

                Loaded = true;

                StateHasChanged();
            }
        }

        private void OnTypeChange(SquadDisplayOption option)
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
            if (int.TryParse(SquadDesignation, out int id))
            {
                Slot = (Slot)id;
                dat = await RosterService.GetSquadDataAsync(Slot, (await AuthService.AuthorizeAsync(user, "RequireManager")).Succeeded);
            }
            else if (SquadDesignation == "me")
            {
                dat = await RosterService.GetSquadDataAsync(user);

                var t = await RosterService.GetTrooperFromClaimsPrincipalAsync(user);
                if (t is not null)
                    Slot = t.Slot;
            }

            if (dat is SquadData d)
                Data = d;
            else if (dat is ZetaSquadData z)
                ZetaData = z;
            else if (dat is ZetaUTCSquadData u)
                ZetaThree = u;
            else if (dat is RazorFlightData r)
                FlightData = r;
            else if (dat is WardenFlightData w)
                WardenData = w;

            await CallRefreshRequest();
        }

        public void Dispose()
        {
            AdvRefreshService.RemoveDataReloadListener(DataReloadRequested);
        }
    }
}