using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperReportPage
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        private TrooperReport Report { get; set; } = new();
        public string FirstReportMessage { get; set; } = "";

        public List<string> Errors { get; set; } = new();
        public string? SuccessMessage { get; set; } = null;

        [Inject]
        public IReportService ReportService { get; set; }

        private async Task OnSubmit()
        {
            if (CurrentUser is null)
            {
                Errors.Add("No trooper is logged in.");
                return;
            }

            ClearErrors();
            ClearSuccess();

            if (string.IsNullOrWhiteSpace(FirstReportMessage))
                Errors.Add("Report message can not be blank");

            if (string.IsNullOrWhiteSpace(Report.Summary))
                Errors.Add("Report summary can not be blank");

            if (Errors.Count > 0)
                return;

            Report.Responses.Add(FirstReportMessage);

            var res = await ReportService.CreateReportAsync(CurrentUser.Id, Report);

            if(!res.GetResult(out var err))
            {
                Errors = err;
            }
            else
            {
                SuccessMessage = "Report submitted.";

                Report = new();
                FirstReportMessage = "";
            }
        }

        private async Task OnCancel()
        {
            Report = new();
            FirstReportMessage = "";
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
