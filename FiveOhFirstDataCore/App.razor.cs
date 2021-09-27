using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore;

public partial class App : IDisposable
{
    private bool disposedValue;

    [Inject]
    public IAlertService AlertService { get; set; }

    private List<AlertData> Alerts { get; set; } = new();
    private RenderFragment? ActiveModal { get; set; } = null;
    private int Counter { get; set; } = 0;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        AlertService.OnAlertPosted += OnAlertPosted;
        AlertService.OnModalPosted += OnModalPosted;
    }

    private void OnAlertPosted(object? sender, AlertData e)
    {
        Alerts.Add(e);
        StateHasChanged();
    }

    private void OnModalPosted(object? sender, RenderFragment modalContent)
    {
        ActiveModal = modalContent;
        StateHasChanged();
    }

    private void OnAlertRemoved(int index)
    {
        Alerts.RemoveAt(index);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                AlertService.OnAlertPosted -= OnAlertPosted;
                AlertService.OnModalPosted -= OnModalPosted;
            }

            Alerts = null;
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~App()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
