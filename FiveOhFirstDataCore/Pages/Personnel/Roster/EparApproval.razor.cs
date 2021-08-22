using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Personnel.Roster
{
    public partial class EparApproval
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IEparService Epar { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [CascadingParameter]
        public Trooper? CurrentTrooper { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [Parameter]
        public string? IdRaw { get; set; }

        private Guid? _guid { get; set; } = null;
        private Guid? Id
        {
            get
            {
                if (IdRaw is null) return null;

                if (_guid is null)
                {
                    try
                    {
                        _guid = new Guid(IdRaw);
                    }
                    catch
                    {
                        return null;
                    }
                }

                return _guid;
            }
        }
        
        private TrooperChangeRequestData? _data { get; set; }
        private TrooperChangeRequestData? Data
        {
            get
            {
                if (Id is null)
                    _data = null;

                return _data;
            }
            set
            {
                _data = value;
            }
        }

        protected List<TrooperChangeRequestData> ChangeRequests { get; set; } = new();

        protected List<string> Errors { get; set; } = new();
        protected string? SuccessMessage { get; set; } = null;

        public List<(string, string)> Urls { get; set; } = new() { ("/", "Home"), ("/c1", "C-1 PERSONNEL"), ("/c1/roster", "Roster Staff Home"),
            ("/c1/roster/epar", "EPAR Home") };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await base.InitalizeAsync(Epar.GetActiveChangeRequests, Epar.GetActiveChangeRequestCount, 15);

                StateHasChanged();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if(Id is not null)
            {
                Data = await Epar.GetChangeRequestAsync(Id.Value);
            }
            else
            {
                Data = null;
            }
        }

        private async Task ApproveChangeAsync()
        {
            if(Data is not null && CurrentTrooper is not null)
            {
                ClearErrors();

                var res = await Epar.FinalizeChangeRequest(Data.ChangeId, true, CurrentTrooper.Id, (await AuthStateTask).User);

                if (!res.GetResult(out var err))
                    Errors.AddRange(err);
                else
                {
                    SuccessMessage = $"Change request approved.";
                    if (!string.IsNullOrWhiteSpace(Data.AdditionalChanges))
                        SuccessMessage += $" Please note the additional changes:\n\n{Data.AdditionalChanges}";
                }
            }

            await base.SetPage(1);
            Nav.NavigateTo("/c1/roster/epar");
        }

        private async Task DenyChangeAsync()
        {
            if (Data is not null && CurrentTrooper is not null)
            {
                ClearErrors();

                var res = await Epar.FinalizeChangeRequest(Data.ChangeId, false, CurrentTrooper.Id, (await AuthStateTask).User);

                if (!res.GetResult(out var err))
                    Errors.AddRange(err);
                else SuccessMessage = "Change request denied.";
            }

            await base.SetPage(1);
            Nav.NavigateTo("/c1/roster/epar");
        }

        private void ClearErrors()
        {
            Errors.Clear();
        }

        private void ClearSuccess()
        {
            SuccessMessage = null;
        }
    }
}
