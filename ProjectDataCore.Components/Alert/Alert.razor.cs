using ProjectDataCore.Data.Structures.Model.Alert;
using ProjectDataCore.Data.Services.Alert;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace ProjectDataCore.Components.Alert
{
    public partial class Alert : ComponentBase
    {
        [Parameter]
        public AlertModel AlertItem { get; set; }

        [Inject]
        public IAlertService AlertService { get; set; }


        private string _alertColor { get {
                switch (AlertItem.AlertType)
                {
                    case AlertType.Success: return "Success";
                    case AlertType.Warn: return "Warn";
                    case AlertType.Error: return "Error";
                    case AlertType.Info: return "Info";
                    default: return "";
                }
            } }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (AlertItem.IsRunning)
            {
                AlertItem.Timer.Start();
                AlertItem.Stopwatch.Start();
            }
        }

        private void CloseAlert()
        {
            AlertService.DeleteAlert(AlertItem);
        }

        private void OnMouseEnter()
        {
            //Console.WriteLine("Mouse Enter");
            if (AlertItem.IsRunning)
            {
                AlertItem.Stopwatch.Stop();
                AlertItem.Timer.Stop();
            }
        }

        private void OnMouseLeave()
        {
            //Console.WriteLine("Mouse Leave");
            if (AlertItem.IsRunning)
            {
                AlertItem.Timer.Interval = AlertItem.Stopwatch.ElapsedMilliseconds;
                AlertItem.Timer.Start();
                AlertItem.Stopwatch.Start();
            }
        }
    }
}
