
using System;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IAdvancedRefreshService
    {
        #region Refresh Requests
        public void AddRefreshListener(string key, Func<Task> refreshRequest);
        public void AddUserSpecificRefreshListener(int? id, string key, Func<Task> refreshRequest);
        public void RemoveRefreshListeners(Func<Task> refreshRequest);

        public void CallRefreshRequest(string key);
        public void CallRefreshRequest(string key, int? idFilter);
        #endregion

        #region Data Requests
        public void AddDataReloadListener(string key, Func<Task> refreshRequest);
        public void AddUserSpecificDataReloadListener(int? id, string key, Func<Task> refreshRequest);
        public void RemoveDataReloadListener(Func<Task> refreshRequest);

        public void CallDataReloadRequest(string key);
        public void CallDataReloadRequest(string key, int? idFilter);
        #endregion

    }
}