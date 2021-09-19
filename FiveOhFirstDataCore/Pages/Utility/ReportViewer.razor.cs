using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using System.ComponentModel;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class ReportViewer
    {
        [Inject]
        public IReportService ReportService { get; set; }
        [Inject]
        public INotificationService NotificationService { get; set; }

        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; }

        protected TrooperReport Report { get; set; }

        private Selection ReportListing { get; set; } = Selection.Mine;

        private IReadOnlyDictionary<Guid, int>? Notifications { get; set; } = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitPagination();
                StateHasChanged();
            }
        }

        private async Task InitPagination()
        {
            switch (ReportListing)
            {
                case Selection.Viewable:
                    var user = (await AuthStateTask).User;
                    var fullAccess = user.IsInRole("Manager") || user.IsInRole("Admin") || user.IsInRole("MP");

                    object[] args = Array.Empty<object>();
                    if (!fullAccess)
                        args = new object[] { CurrentUser?.Slot ?? Slot.Archived };

                    await base.InitalizeAsync(ReportService.GetTrooperReportsAsync,
                        ReportService.GetTrooperReportCountsAsync, args, 15);
                    break;
                case Selection.Notifying:
                    await base.InitalizeAsync(ReportService.GetNotifyingReportsAsync,
                        ReportService.GetNotifyingReportCountsAsync,
                        new object[] { CurrentUser?.Id ?? 0 }, 15);
                    break;
                case Selection.Participated:
                    await base.InitalizeAsync(ReportService.GetParticipatingReportsAsync,
                        ReportService.GetParticipatingReportCountsAsync,
                        new object[] { CurrentUser?.Id ?? 0 }, 15);
                    break;

                default:
                case Selection.Mine:
                    await base.InitalizeAsync(ReportService.GetPersonalReportsAsync,
                        ReportService.GetPersonalReportCountsAsync,
                        new object[] { CurrentUser?.Id ?? 0 }, 15);
                    break;
            }

            List<Guid> ids = new();
            foreach (var i in Items)
                ids.Add(((TrooperReport)i).Id);

            Notifications = await NotificationService
                .GetReportNotificationCountsAsync(CurrentUser?.Id ?? 0, ids.ToArray());
        }

        private int GetNotificationCount(Guid id)
        {
            int count = 0;
            _ = Notifications?.TryGetValue(id, out count);

            return count;
        }

        private async Task SelectionChanged(ChangeEventArgs e)
        {
            ReportListing = Enum.Parse<Selection>((string?)e.Value ?? "");
            await InitPagination();
        }

        private enum Selection
        {
            [Description("My Reports")]
            Mine,
            [Description("Viewable Reports")]
            Viewable,
            [Description("Participated Reports")]
            Participated,
            [Description("Notifying Reports")]
            Notifying
        }
    }
}
