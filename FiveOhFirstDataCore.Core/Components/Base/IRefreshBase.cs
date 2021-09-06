namespace FiveOhFirstDataCore.Core.Components.Base
{
    public interface IRefreshBase : IDisposable
    {
        public Task CallRefreshRequest();
    }
}
