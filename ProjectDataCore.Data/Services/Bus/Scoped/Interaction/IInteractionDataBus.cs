using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial interface IScopedDataBus
{
    public delegate Task PageClickedEventHandler(object sender, PageClickedEventArgs args);
    public event PageClickedEventHandler PageClicked;
    public Task SendPageClickEventAsync(object sender, PageClickedEventArgs args);

    public delegate Task KeyPressedEventHandler(object sender, KeyboardEventArgs args);
    public event KeyPressedEventHandler KeyPressed;
    public Task SendKeyPressEventAsync(object sender, KeyboardEventArgs args);
}
