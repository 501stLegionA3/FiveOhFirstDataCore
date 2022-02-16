using ProjectDataCore.Data.Structures.Model.Alert;
using System.Timers;

namespace ProjectDataCore.Data.Services.Alert
{
    public class AlertService : IAlertService
    {
        public List<AlertModel> Alerts { get; set; } = new();
        public event EventHandler AlertsChanged;

        public ActionResult CreateAlert(string message, AlertType alertType, bool enableTimer, int duration, bool trigger = true)
        {
            AlertModel alertModel = new(alertType, message, this, enableTimer, duration);
            Alerts.Add(alertModel);
            if (trigger)
                return TriggerEvent();
            return new(true);
        }

        public ActionResult AddAlert(AlertModel alert)
        {
            Alerts.Add(alert);
            return TriggerEvent();
        }

        public ActionResult CreateErrorAlert(string message, bool enableTimer = false, int duration = 5000)
        {
            return CreateAlert(message, AlertType.Error, enableTimer, duration);
        }

        public ActionResult CreateErrorAlert(List<string> messages, bool enableTimer = false, int duration = 5000)
        {
            messages.ForEach(x => CreateAlert(x, AlertType.Error, enableTimer, duration, false));
            return TriggerEvent();
        }

        public ActionResult CreateInfoAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Info, enableTimer, duration);
        }

        public ActionResult CreateInfoAlert(List<string> messages, bool enableTimer = false, int duration = 3200)
        {
            messages.ForEach(x => CreateAlert(x, AlertType.Info, enableTimer, duration, false));
            return TriggerEvent();
        }

        public ActionResult CreateSuccessAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Success, enableTimer, duration);
        }

        public ActionResult CreateSuccessAlert(List<string> messages, bool enableTimer = false, int duration = 3200)
        {
            messages.ForEach(x => CreateAlert(x, AlertType.Success, enableTimer, duration, false));
            return TriggerEvent();
        }

        public ActionResult CreateWarnAlert(string message, bool enableTimer = false, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Warn, enableTimer, duration);
        }
        public ActionResult CreateWarnAlert(List<string> messages, bool enableTimer = false, int duration = 3200)
        {
            messages.ForEach(x => CreateAlert(x, AlertType.Warn, enableTimer, duration, false));
            return TriggerEvent();
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
