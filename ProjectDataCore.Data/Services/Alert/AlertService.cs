using ProjectDataCore.Data.Structures.Model.Alert;
using System.Timers;

namespace ProjectDataCore.Data.Services.Alert
{
    public class AlertService : IAlertService
    {
        public List<AlertModel> Alerts { get; set; } = new();
        public event EventHandler AlertsChanged;

        public ActionResult CreateAlert(string message, AlertType alertType, bool enableTimer, int duration)
        {
            AlertModel alertModel = new(alertType, message, this, enableTimer, duration);
            Alerts.Add(alertModel);
            return TriggerEvent();
        }

        public ActionResult CreateErrorAlert(string message, bool enableTimer = false, int duration = 5000)
        {
            return CreateAlert(message, AlertType.Error, enableTimer, duration);
        }

        public ActionResult CreateInfoAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Info, enableTimer, duration);
        }

        public ActionResult CreateSuccessAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Success, enableTimer, duration);
        }

        public ActionResult CreateWarnAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Warn, enableTimer, duration);
        }

        public ActionResult DeleteAlert(AlertModel alert)
        {
            alert.Dispose();
            Alerts.Remove(alert);
            return TriggerEvent();
        }

        internal ActionResult TriggerEvent()
        {
            try
            {
                AlertsChanged?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message});
            }
            return new(true);
        }


    }
}
