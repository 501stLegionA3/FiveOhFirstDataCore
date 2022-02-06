using ProjectDataCore.Data.Structures.Model.Alert;

namespace ProjectDataCore.Data.Services.Alert
{
    public interface IAlertService
    {
        public List<AlertModel> Alerts { get; set;}
        public event EventHandler AlertsChanged;

        /// <summary>
        /// Generic method used to create a new alert of any type
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="alertType">One of <see cref="AlertType"/> to determine severity</param>
        /// <param name="enableTimer">If True the timer will be eneabled.</param>
        /// <param name="duration">How long the alert will display in ms</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult CreateAlert(string message, AlertType alertType, bool enableTimer, int duration);
        /// <summary>
        /// Create a Seccess Alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="enableTimer">If True the timer will be eneabled.</param>
        /// <param name="duration">How long the alert will display in ms</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult CreateSuccessAlert(string message, bool enableTimer = false, int duration = 3200);
        /// <summary>
        /// Create an Error Alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="enableTimer">If True the timer will be eneabled.</param>
        /// <param name="duration">How long the alert will display in ms</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult CreateErrorAlert(string message, bool enableTimer = false, int duration = 5000);
        /// <summary>
        /// Create an Info Alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="enableTimer">If True the timer will be eneabled.</param>
        /// <param name="duration">How long the alert will display in ms</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult CreateInfoAlert(string message, bool enableTimer = false, int duration = 3200);
        /// <summary>
        /// Create a Warning Alert
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        /// <param name="enableTimer">If True the timer will be eneabled.</param>
        /// <param name="duration">How long the alert will display in ms</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult CreateWarnAlert(string message, bool enableTimer = false, int duration = 3200);
        /// <summary>
        /// Delete an alert based on its Id
        /// </summary>
        /// <param name="alert">The alert to delete.</param>
        /// <returns>True/False depedning on errors</returns>
        public ActionResult DeleteAlert(AlertModel alert);
    }
}
