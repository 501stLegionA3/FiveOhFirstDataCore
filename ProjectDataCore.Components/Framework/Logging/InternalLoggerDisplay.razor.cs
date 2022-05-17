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
    [Parameter]
    public Action? RefreshCalled { get; set; }

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
        if(RefreshCalled is not null)
            RefreshCalled.Invoke();
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