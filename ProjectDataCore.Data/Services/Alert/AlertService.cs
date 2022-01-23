using ProjectDataCore.Data.Structures.Model.Alert;
using System.Timers;

namespace ProjectDataCore.Data.Services.Alert
{
    public class AlertService : IAlertService
    {
        public List<AlertModel> Alerts { get; set; } = new();
        public event EventHandler AlertsChanged;

        public ActionResult CreateAlert(string message, AlertType alertType, int duration)
        {
            AlertModel alertModel = new(alertType, message);
            Alerts.Add(alertModel);
            System.Timers.Timer timer = new(duration);
            timer.Elapsed += (sender, e)=>DeleteAlert(alertModel.Id);
            //timer.Start(); TODO: Renable
            return TriggerEvent();
        }

        public ActionResult CreateErrorAlert(string message, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Error, duration);
        }

        public ActionResult CreateInfoAlert(string message, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Info, duration);
        }

        public ActionResult CreateSuccessAlert(string message, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Success, duration);
        }

        public ActionResult CreateWarnAlert(string message, int duration = 3200)
        {
            return CreateAlert(message, AlertType.Warn, duration);
        }

        public ActionResult DeleteAlert(Guid alertId)
        {
            Alerts.RemoveAll(e => e.Id.Equals(alertId));
            return TriggerEvent();
        }

        internal ActionResult TriggerEvent()
        {
            try
            {
                AlertsChanged.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message});
            }
            return new(true);
        }


    }
}
