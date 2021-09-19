namespace FiveOhFirstDataCore.Data.Components.Base
{
    public interface IRefreshBase : IDisposable
    {
        public Task CallRefreshRequest();
    }
}
