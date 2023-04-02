namespace FiveOhFirstDataCore.Pages.Roster
{
    public partial class RosterHome
    {
        public enum RosterType
        {
            Orbat,
            Roster,
            ReserveOrbat,
            ReserveRoster,
            InactiveReserves,
            FullRoster
        }

        public RosterType Active { get; set; } = RosterType.Orbat;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            _refresh.RefreshRequested += RefreshMe;
        }

        private void RefreshMe()
        {
            InvokeAsync(() => StateHasChanged());
        }

        protected void OnTypeChange(RosterType newType)
        {
            Active = newType;
            _refresh.CallRequestRefresh();
        }

        void IDisposable.Dispose()
            => _refresh.RefreshRequested -= RefreshMe;
    }
}
