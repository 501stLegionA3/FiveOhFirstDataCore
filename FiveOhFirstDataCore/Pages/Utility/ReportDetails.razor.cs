using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class ReportDetails
    {
        [Parameter]
        public string ReportId {  get; set; }

        private Guid? Id
        {
            get {
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await LoadReportData();
                StateHasChanged();
            }
        }

        private async Task LoadReportData()
        {
            var user = (await AuthStateTask).User;
            var manager = user.IsInRole("Manager") || user.IsInRole("Admin");


        }
    }
}
