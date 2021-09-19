using System.IO;

namespace FiveOhFirstDataCore.Data.Structures.Import
{
    public class OrbatImport : IDisposable, IAsyncDisposable
    {
        private bool disposedValue;

        public FileStream? RosterStream { get; set; }
        public FileStream? ZetaRosterStream { get; set; }
        public FileStream? CShopStream { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RosterStream?.Dispose();
                    ZetaRosterStream?.Dispose();
                    CShopStream?.Dispose();
                }

                RosterStream = null;
                ZetaRosterStream = null;
                CShopStream = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (RosterStream is not null)
                await RosterStream.DisposeAsync();

            if (ZetaRosterStream is not null)
                await ZetaRosterStream.DisposeAsync();

            if (CShopStream is not null)
                await CShopStream.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
