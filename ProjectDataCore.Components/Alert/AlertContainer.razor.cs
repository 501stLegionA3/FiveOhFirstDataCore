using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Structures.Model.Alert;

namespace ProjectDataCore.Components.Alert
{
    public partial class AlertContainer : ComponentBase, IDisposable
    {
        [Inject]
        public IAlertService AlertService { get; set; }

        private List<AlertModel> _alerts = new();

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _alerts = AlertService.Alerts;
            AlertService.AlertsChanged += AlertsChanged;
        }

        public void AlertsChanged(object sender, EventArgs e)
        {
            _alerts = AlertService.Alerts;
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            AlertService.AlertsChanged -= AlertsChanged;
        }
    }
}
