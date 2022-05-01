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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private double _xPos = 0;
    private double _yPos = 0;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        _xPos = EventArgs.ScreenX;
        _yPos = EventArgs.ScreenY;
    }


}
