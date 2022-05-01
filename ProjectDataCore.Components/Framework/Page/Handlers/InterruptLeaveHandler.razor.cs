using ProjectDataCore.Data.Services.Bus.Scoped;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class InterruptLeaveHandler
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public NavigationManager NavManager { get; set; }
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // All dis is TODO - we require .NET 7 preview 5 as of right now.
        // Eta - June ish
        // https://github.com/dotnet/aspnetcore/issues/14962
    }


}
