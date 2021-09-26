// Do I Need Too? - Cruisie

namespace FiveOhFirstDataCore.Pages.Manager
{
    public partial class ManagerSettings
    {
        public List<(string, string)> Urls = new() { ("/", "Home"), ("/manager", "Manager Home"), ("/manager/settings", "Manager Settings") };

        public enum ManagerDisplayOption
        {
            CShops,
            Discord,
            Promotions,
            Settings,
        }

        public ManagerDisplayOption Active { get; set; }

        private void OnTypeChange(ManagerDisplayOption option)
        {
            Active = option;
            StateHasChanged();
        }

        protected bool IsAcknowledgedDWS { get; set; }
        protected bool IsAcknowledgedVTR { get; set; }
    }
}
