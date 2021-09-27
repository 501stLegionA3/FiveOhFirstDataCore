using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IAlertService
    {
        /// <summary>
        /// Fires when a new alert is posted.
        /// </summary>
        public event EventHandler<AlertData> OnAlertPosted;
        /// <summary>
        /// Fires when a new modal is posted.
        /// </summary>
        public event EventHandler<RenderFragment> OnModalPosted;
        /// <summary>
        /// Post a new alert.
        /// </summary>
        /// <param name="sender">Object posting this alert.</param>
        /// <param name="alert">Alert to post.</param>
        public void PostAlert(object sender, AlertData alert);
        /// <summary>
        /// Post a new error alert.
        /// </summary>
        /// <param name="sender">Object posting this alert.</param>
        /// <param name="errors">A list of error messages to post.</param>
        public void PostAlert(object sender, List<string> errors);
        /// <summary>
        /// Post a new success alert.
        /// </summary>
        /// <param name="sender">Object posting this alert.</param>
        /// <param name="success">The success message to post.</param>
        public void PostAlert(object sender, string success);
        /// <summary>
        /// Post a new modal.
        /// </summary>
        /// <param name="sender">Object posting this modal.</param>
        /// <param name="modalContent">The modal content to post.</param>
        public void PostModal(object sender, RenderFragment modalContent);
    }
}
