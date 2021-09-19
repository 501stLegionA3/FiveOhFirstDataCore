using System.Collections.Concurrent;

namespace FiveOhFirstDataCore.Data.Services
{
    public class AdvancedRefreshService : IAdvancedRefreshService
    {
        protected ConcurrentDictionary<string, ConcurrentDictionary<Func<Task>, int?>> ActiveRefreshListeners { get; init; }
        protected ConcurrentDictionary<string, ConcurrentDictionary<Func<Task>, int?>> DataReloadListeners { get; set; }

        public AdvancedRefreshService()
        {
            ActiveRefreshListeners = new();
            DataReloadListeners = new();
        }

        public void AddRefreshListener(string key, Func<Task> refreshRequest)
            => AddUserSpecificRefreshListener(null, key, refreshRequest);

        public void AddUserSpecificRefreshListener(int? id, string key, Func<Task> refreshRequest)
        {
            if (ActiveRefreshListeners.TryGetValue(key, out var data))
                data[refreshRequest] = id;
            else
                ActiveRefreshListeners[key] = new() { [refreshRequest] = id };
        }

        public void RemoveRefreshListeners(Func<Task> refreshRequest)
        {
            foreach (var set in ActiveRefreshListeners)
                _ = set.Value.TryRemove(refreshRequest, out _);
        }

        public void CallRefreshRequest(string key)
            => CallRefreshRequest(key, null);

        public void CallRefreshRequest(string key, int? idFilter)
        {
            foreach (var data in ActiveRefreshListeners)
            {
                if (data.Key == key)
                {
                    foreach (var pair in data.Value)
                    {
                        if (idFilter is null || pair.Value == idFilter)
                        {
                            try
                            {
                                pair.Key();
                            }
                            catch
                            {
                                // Not our problem
                            }
                        }
                    }
                }
            }
        }

        public void AddDataReloadListener(string key, Func<Task> refreshRequest)
            => AddUserSpecificDataReloadListener(null, key, refreshRequest);

        public void AddUserSpecificDataReloadListener(int? id, string key, Func<Task> refreshRequest)
        {
            if (DataReloadListeners.TryGetValue(key, out var data))
                data[refreshRequest] = id;
            else
                DataReloadListeners[key] = new() { [refreshRequest] = id };
        }

        public void RemoveDataReloadListener(Func<Task> refreshRequest)
        {
            foreach (var set in DataReloadListeners)
                _ = set.Value.TryRemove(refreshRequest, out _);
        }

        public void CallDataReloadRequest(string key)
            => CallDataReloadRequest(key, null);

        public void CallDataReloadRequest(string key, int? idFilter)
        {
            foreach (var data in DataReloadListeners)
            {
                if (data.Key == key)
                {
                    foreach (var pair in data.Value)
                    {
                        if (idFilter is null || pair.Value == idFilter)
                        {
                            try
                            {
                                pair.Key();
                            }
                            catch
                            {
                                // Not our problem
                            }
                        }
                    }
                }
            }
        }
    }
}