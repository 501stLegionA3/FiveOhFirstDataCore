using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using ProjectDataCore.Components.Framework;
using ProjectDataCore.Components.Framework.Selectors;
using ProjectDataCore.Components.Framework.Select;
using ProjectDataCore.Components.Framework.Component;
using ProjectDataCore.Components.Roster;
using ProjectDataCore.Components.Parts.Roster.Parts;
using ProjectDataCore.Components.Util;
using ProjectDataCore.Data.Services.Roster;
using ProjectDataCore.Data.Structures;
using ProjectDataCore.Data.Structures.Assignable;
using ProjectDataCore.Data.Structures.Model;
using ProjectDataCore.Data.Structures.Result;
using ProjectDataCore.Data.Structures.Util;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Nav;
using ProjectDataCore.Data.Structures.Logging;
using Microsoft.Extensions.Configuration;
using ProjectDataCore.Data.Services.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ProjectDataCore.Components.Framework.Logging;
public partial class InternalLoggerDisplay : IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IConfiguration Configuration { get; set; }
    [Inject]
    public IInstanceLogger InstanceLogger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public string Title { get; set; }
    [Parameter]
    public Guid Scope { get; set; }
    [Parameter]
    public CancellationTokenSource? CancellationSource { get; set; }

    public List<DataCoreLog> Logs { get; set; } = new();

    private LogLevel LoggerLevel { get; set; } = LogLevel.Information;
    private ConcurrentDictionary<LogLevel, bool> ActiveScopes { get; init; } = new(new Dictionary<LogLevel, bool>()
        {
            { LogLevel.Critical, true },
            { LogLevel.Error, true },
            { LogLevel.Warning, true },
            { LogLevel.Information, true },
            { LogLevel.Debug, true },
        });

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        try
        {
            var logLevel = Configuration["Logging:LogLevel:Instance"];
            LoggerLevel = Enum.Parse<LogLevel>(logLevel);
        }
        catch
        {
            LoggerLevel = LogLevel.Information;
        }

        InstanceLogger.Register(LogMessageReceived, LoggerLevel, Scope);
    }

    protected void LogMessageReceived(DataCoreLog log)
    {
        Logs.Add(log);
        StateHasChanged();
    }

    protected void ToggleLogDisplay(LogLevel level)
    {
        if (ActiveScopes.TryGetValue(level, out var val))
            ActiveScopes[level] = !val;
    }

    public void Dispose()
    {
        InstanceLogger.Unregister(LogMessageReceived);
    }
}