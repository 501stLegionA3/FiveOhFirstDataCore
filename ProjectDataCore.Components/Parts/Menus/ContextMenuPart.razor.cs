using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Page.ContextMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Menus;
/// <summary>
/// Displays a context menu.
/// </summary>
public partial class ContextMenuPart : PopupMenuBase
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IJSRuntime JSRuntime;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private class WH
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    private bool _left = true;
    private double _xPos = 0;

    private bool _top = true;
    private double _yPos = 0;

    private bool _positionReady = false;
    private bool _displayReady = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            var wh = await JSRuntime.InvokeAsync<WH>("Util.getWindowDimensions");

            AdjustPosition(wh);

            _positionReady = true;
            StateHasChanged();
        }

        if(_positionReady)
        {
            _displayReady = true;
            StateHasChanged();
        }
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

        _xPos = EventArgs.ScreenX;
        _yPos = EventArgs.ScreenY;
    }

    private void AbortMenu()
    {
        ScopedDataBus.ClosePopupMenu(this);
    }
}
