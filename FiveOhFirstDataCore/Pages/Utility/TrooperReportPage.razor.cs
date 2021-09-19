using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperReportPage
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        private TrooperReport Report { get; set; } = new();
        public string FirstReportMessage { get; set; } = "";

        [Inject]
        public IReportService ReportService { get; set; }
        [Inject]
        public IAlertService AlertService { get; set; }

        private async Task OnSubmit()
        {
            var errors = new List<string>();
            if (CurrentUser is null)
            {
                errors.Add("No trooper is logged in.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstReportMessage))
                errors.Add("Report message can not be blank");

            if (string.IsNullOrWhiteSpace(Report.Summary))
                errors.Add("Report summary can not be blank");

            if (errors.Count > 0)
            {
                AlertService.PostAlert(this, errors);
                return;
            }

            Report.Responses.Add(new()
            {
                Message = FirstReportMessage,
                CreatedOn = DateTime.UtcNow,
                AuthorId = CurrentUser.Id
            });

            var res = await ReportService.CreateReportAsync(CurrentUser.Id, Report);

            if (!res.GetResult(out var err))
            {
                AlertService.PostAlert(this, err);
            }
            else
            {
                AlertService.PostAlert(this, "Report submitted.");

                Report = new();
                FirstReportMessage = "";
            }
        }

        private async Task OnCancel()
        {
            Report = new();
            FirstReportMessage = "";
        }
    }
}
