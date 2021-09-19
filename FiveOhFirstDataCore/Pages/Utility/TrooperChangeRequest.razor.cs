using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperChangeRequest
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        public TrooperChangeRequestData Model { get; set; } = new();

        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IEparService Epar { get; set; }
        [Inject]
        public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                Model.Qualifications = CurrentUser?.Qualifications ?? Qualification.None;
                StateHasChanged();
            }
        }

        private async Task OnSubmit()
        {
            List<string> errors = new();
            if (Model.Reason is null)
            {
                errors.Add("You must include a reason in your request.");
            }

            if (CurrentUser is null)
            {
                errors.Add("You must be signed in.");
            }
            else if (!(Model.HasChange() || Model.Qualifications != CurrentUser.Qualifications))
            {
                errors.Add("You must have at least one change in your request.");
            }

            if (errors.Count > 0)
            {
                AlertService.PostAlert(this, errors);
                return;
            }

            var res = await Epar.CreateChangeRequest(Model, CurrentUser!.Id);

            if (!res.GetResult(out var err))
            {
                AlertService.PostAlert(this, err);
            }
            else
            {
                Model = new()
                {
                    Qualifications = CurrentUser!.Qualifications
                };
                AlertService.PostAlert(this, "Change request has been submitted.");
            }
        }
    }
}
