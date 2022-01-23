using ProjectDataCore.Data.Structures.Model.Alert;
using ProjectDataCore.Data.Services.Alert;

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


    private void CloseAlert()
        {
            AlertService.DeleteAlert(AlertItem.Id);
        }
    }
}
