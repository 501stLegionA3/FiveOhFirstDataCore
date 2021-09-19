using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FiveOhFirstDataCore.Pages.Personnel.Roster
{
    public partial class EparApproval
    {
        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IEparService Epar { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }
        [Inject]
        public IAlertService AlertService { get; set; }

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
                else if (_guid.ToString() != IdRaw)
                {
                    _guid = new Guid(IdRaw);
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
        public List<(string, string)> Urls { get; set; } = new()
        {
            ("/", "Home"),
            ("/c1", "C-1 PERSONNEL"),
            ("/c1/roster", "Roster Staff Home"),
            ("/c1/roster/epar", "EPAR Home")
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (Id is not null)
            {
                Data = await Epar.GetChangeRequestAsync(Id.Value);
            }
            else
            {
                Data = null;
            }

            await base.InitalizeAsync(Epar.GetActiveChangeRequests, Epar.GetActiveChangeRequestCount, itemsPerPage: 15);

            StateHasChanged();
        }

        private async Task ApproveChangeAsync()
        {
            if (Data is not null && CurrentTrooper is not null)
            {
                var res = await Epar.FinalizeChangeRequest(Data.ChangeId, true, CurrentTrooper.Id, (await AuthStateTask).User);

                if (!res.GetResult(out var err))
                    AlertService.PostAlert(this, err);
                else
                {
                    var successMessage = $"Change request approved.";
                    if (!string.IsNullOrWhiteSpace(Data.AdditionalChanges))
                        successMessage += $" Please note the additional changes:\n\n{Data.AdditionalChanges}";
                    AlertService.PostAlert(this, successMessage);
                }
            }

            await base.SetPage(1);
            Nav.NavigateTo("/c1/roster/epar");
        }

        private async Task DenyChangeAsync()
        {
            if (Data is not null && CurrentTrooper is not null)
            {
                var res = await Epar.FinalizeChangeRequest(Data.ChangeId, false, CurrentTrooper.Id, (await AuthStateTask).User);

                if (!res.GetResult(out var err))
                    AlertService.PostAlert(this, err);
                else
                    AlertService.PostAlert(this, "Change request denied.");
            }

            await base.SetPage(1);
            Nav.NavigateTo("/c1/roster/epar");
        }
    }
}
