namespace FiveOhFirstDataCore.Data.Services
{
    public interface IRefreshRequestService
    {
        /// <summary>
        /// An <see cref="Action"/> that is called when a refresh is requested.
        /// </summary>
        public event Action RefreshRequested;
        /// <summary>
        /// Trigger the <see cref="IRefreshRequestService.RefreshRequested"/> <see cref="Action"/>.
        /// </summary>
        public void CallRequestRefresh();
    }
}
