using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial class ScopedDataBus : IScopedDataBus
{
    public event IScopedDataBus.PageClickedEventHandler? PageClicked;
    public Task SendPageClickEventAsync(object sender, PageClickedEventArgs args)
    {
        if(PageClicked is not null)
            _ = Task.Run(async () => await PageClicked.Invoke(sender, args));

        return Task.CompletedTask;
    }
}
