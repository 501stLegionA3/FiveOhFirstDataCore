namespace FiveOhFirstDataCore.Data.Services
{
    public interface IAdvancedRefreshService
    {
        #region Refresh Requests
        /// <summary>
        /// Add a new standard refresh listener.
        /// </summary>
        /// <param name="key">String based key. Matching keys will be called when a refresh is requested.</param>
        /// <param name="refreshRequest">The task that will be triggered when a refresh is called.</param>
        public void AddRefreshListener(string key, Func<Task> refreshRequest);
        /// <summary>
        /// Add a new user specific refresh listener.
        /// </summary>
        /// <param name="id">The ID of the users to listen to refreshs for.</param>
        /// <param name="key">String based key. Matching keys will be called when a refresh is requested.</param>
        /// <param name="refreshRequest">The task that will be triggered when a refresh is called.</param>
        public void AddUserSpecificRefreshListener(int? id, string key, Func<Task> refreshRequest);
        /// <summary>
        /// Remove a method from the list of refresh listeners.
        /// </summary>
        /// <param name="refreshRequest">The registered method to remove.</param>
        public void RemoveRefreshListeners(Func<Task> refreshRequest);

        /// <summary>
        /// Calls a standard refresh request.
        /// </summary>
        /// <param name="key">String based key. All listeners with this key will be refreshed.</param>
        public void CallRefreshRequest(string key);
        /// <summary>
        /// Calls a user specific refresh request.
        /// </summary>
        /// <param name="key">String based key. All listeners with this key will be refreshed.</param>
        /// <param name="idFilter">User to call this refresh request for.</param>
        public void CallRefreshRequest(string key, int? idFilter);
        #endregion

        #region Data Requests
        /// <summary>
        /// Add a new data reload request listener.
        /// </summary>
        /// <remarks>
        /// The data reload request listener is used for requesting other pages to reload their data. This includes
        /// asking pages to make new calls to get updated database information.
        /// </remarks>
        /// <param name="key">String based key. All data listeners with this key will be refreshed.</param>
        /// <param name="refreshRequest">The task to run when a data reload is requested.</param>
        public void AddDataReloadListener(string key, Func<Task> refreshRequest);
        /// <summary>
        /// Add a new data reload request listener specific to a user.
        /// </summary>
        /// <remarks>
        /// The data reload request listener is used for requesting other pages to reload their data. This includes
        /// asking pages to make new calls to get updated database information. With the inclusion of a user ID,
        /// this listener will only be called when a reload request is made with a matching user ID.
        /// </remarks>
        /// <param name="id">The ID of the user to listen to refreshs for.</param>
        /// <param name="key">String based key. All data listeners with this key will be refreshed.</param>
        /// <param name="refreshRequest">The task to run when a data reload is requested.</param>
        public void AddUserSpecificDataReloadListener(int? id, string key, Func<Task> refreshRequest);
        /// <summary>
        /// Remove a method from the list of data reload listeners.
        /// </summary>
        /// <param name="refreshRequest">The registered method to remove.</param>
        public void RemoveDataReloadListener(Func<Task> refreshRequest);

        /// <summary>
        /// Call a data reload request.
        /// </summary>
        /// <param name="key">String based key. All data reload listeners with this key will be refreshed.</param>
        public void CallDataReloadRequest(string key);
        /// <summary>
        /// Call a data reload request for a specific user.
        /// </summary>
        /// <param name="key">String based key. All data reload listeners with this key will be refreshed.</param>
        /// <param name="idFilter">The ID of the user to refresh.</param>
        public void CallDataReloadRequest(string key, int? idFilter);
        #endregion

    }
}