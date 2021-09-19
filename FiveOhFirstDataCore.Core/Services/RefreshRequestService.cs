namespace FiveOhFirstDataCore.Data.Services
{
    public class RefreshRequestService : IRefreshRequestService
    {
        public event Action? RefreshRequested;
        public void CallRequestRefresh()
        {
            RefreshRequested?.Invoke();
        }
    }
}
