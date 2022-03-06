using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Structures.Model.Alert;

using System.Collections.Concurrent;
using System.Text.Encodings.Web;

namespace ProjectDataCore.Pages;
public partial class Index
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public string? Route { get; set; }
    private ConcurrentDictionary<string, StringValues> Query { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        Query = new(QueryHelpers.ParseQuery(uri.Query));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            // Handle alert queries.
            if (Query.TryGetValue("alert", out var value))
            {
                var parts = value.ToString().Split(';');
                if (parts.Length == 4)
                {
                    if (Enum.TryParse<AlertType>(parts[1], out var type))
                        if (bool.TryParse(parts[2], out var timer))
                            if (int.TryParse(parts[3], out var duration))
                                AlertService.CreateAlert(parts[0], type, timer, duration);
                }
            }
        }
    }
}