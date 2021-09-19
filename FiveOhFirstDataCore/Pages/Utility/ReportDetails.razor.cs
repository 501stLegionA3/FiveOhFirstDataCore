using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class ReportDetails
    {
        [Parameter]
        public string ReportId { get; set; }

        private Guid? Id
        {
            get
            {
                if (Guid.TryParse(ReportId, out Guid res))
                    return res;
                else return null;
            }
        }

        [Inject]
        public INotificationService NotificationService { get; set; }

        [Inject]
        public IReportService ReportService { get; set; }

        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        public TrooperReport? Report { get; set; } = null;

        private bool IsNotified { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await LoadReportData();
                StateHasChanged();
            }
        }

        private async Task LoadReportData()
        {
            Guid? id = Id;
            if (CurrentUser is not null && id is not null)
            {
                var user = (await AuthStateTask).User;
                var manager = user.IsInRole("Manager") || user.IsInRole("Admin");

                Report = await ReportService.GetTrooperReportIfAuthorized(id.Value, CurrentUser.Id, manager);
                IsNotified = await NotificationService.IsTrooperNotifiedForReportAsync(id.Value, CurrentUser.Id);
            }
        }

        public async Task OnAfterPostAsync()
        {
            if (Report is not null)
            {
                var rid = Report.Id;
                var cid = CurrentUser?.Id ?? 0;
                await NotificationService.UpdateReportViewDateTimeAsync(rid, cid);

                await ReportService.UpdateReportLastUpdateAsync(rid);
            }
        }

        private async Task OnNotifyChanged()
            => IsNotified = await NotificationService.ToggleReportNotificationTracker(Report!.Id, CurrentUser!.Id);

        private async Task OnFinalizeChanged()
            => Report = await ReportService.ToggleResolvedAsync(Report!.Id);

        private async Task OnElevateChanged()
            => Report = await ReportService.ElevateToBattalionAsync(Report!.Id);
    }
}
