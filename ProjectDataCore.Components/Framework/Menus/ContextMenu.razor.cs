using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Page.ContextMenu;

namespace ProjectDataCore.Components.Framework.Menus;
/// <summary>
/// Displays a context menu.
/// </summary>
public partial class ContextMenu : PopupMenuBase, IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public bool CloseWhenOutsideClickOccours { get; set; } = true;

    private class WH
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    private class BoundingBox
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
    }

    private BoundingBox? _boundingBox;
    private bool _left = true;
    private double _xPos = 0;

    private bool _top = true;
    private double _yPos = 0;
    private bool disposedValue;

    private bool _positionReady = false;
    private bool _menuReady = false;
    private readonly Guid _key = Guid.NewGuid();

    private async Task OnMenuLoaded()
    {
        if (!_menuReady)
        {
            _menuReady = true;
        }
    }

    private async Task OnContextMenu(MouseEventArgs args)
    {
        _xPos = args.PageX;
        _yPos = args.PageY;

        var wh = await JSRuntime.InvokeAsync<WH>("Util.getWindowDimensions");

        AdjustPosition(wh);

        _positionReady = true;

        StateHasChanged();
    }

    private void AdjustPosition(WH screen)
    {
        if (_xPos > (screen.Width / 2))
        {
            _xPos = screen.Width - _xPos;
            _left = false;
        }

        if(_yPos > (screen.Height / 2))
        {
            _yPos = screen.Width - _yPos;
            _top = false;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ScopedDataBus.PageClicked += ScopedDataBus_PageClicked;
    }

    private Task ScopedDataBus_PageClicked(object sender, PageClickedEventArgs args)
    {
        _ = Task.Run(async () =>
        {
            if(CloseWhenOutsideClickOccours 
                && _boundingBox is not null)
            {
                if(args.XPos < _boundingBox.Left
                    || args.XPos > _boundingBox.Right
                    || args.YPos < _boundingBox.Top
                    || args.YPos > _boundingBox.Bottom)
                {
                    await AbortMenuAsync();
                }
            }
        });

        return Task.CompletedTask;
    }

    private async Task AbortMenuAsync()
    {
        _positionReady = false;
        _menuReady = false;

        await InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                ScopedDataBus.PageClicked -= ScopedDataBus_PageClicked;
            }

            _boundingBox = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
