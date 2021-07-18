using FiveOhFirstDataCore.Core.Components.Base;
using FiveOhFirstDataCore.Core.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IAdvancedRefreshService
    {
        #region Refresh Requests
        public void AddRefreshListener(RefreshRequestValues key, Func<Task> refreshRequest);
        public void AddUserSpecificRefreshListener(int? id, RefreshRequestValues key, Func<Task> refreshRequest);
        public void RemoveRefreshListeners(Func<Task> refreshRequest);

        public void CallRefreshRequest(RefreshRequestValues key);
        public void CallRefreshRequest(RefreshRequestValues key, int? idFilter);
        #endregion

        #region Data Requests
        public void AddDataReloadListener(RefreshRequestValues key, Func<Task> refreshRequest);
        public void AddUserSpecificDataReloadListener(int? id, RefreshRequestValues key, Func<Task> refreshRequest);
        public void RemoveDataReloadListener(Func<Task> refreshRequest);

        public void CallDataReloadRequest(RefreshRequestValues key);
        public void CallDataReloadRequest(RefreshRequestValues key, int? idFilter);
        #endregion

    }
}
