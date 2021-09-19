using System.IO;

namespace FiveOhFirstDataCore.Data.Structures.Import
{
    public class SupportingElementsImport : IDisposable, IAsyncDisposable
    {
        private bool disposedValue;

        public FileStream? InactiveReservesStream { get; set; }
        public FileStream? TrainingStream { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    InactiveReservesStream?.Dispose();
                    TrainingStream?.Dispose();
                }

                InactiveReservesStream = null;
                TrainingStream = null;

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
            if (InactiveReservesStream is not null)
                await InactiveReservesStream.DisposeAsync();

            if (TrainingStream is not null)
                await TrainingStream.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
