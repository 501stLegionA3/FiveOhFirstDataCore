using System;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IRefreshRequestService
    {
        public event Action RefreshRequested;
        public void CallRequestRefresh();
    }
}
