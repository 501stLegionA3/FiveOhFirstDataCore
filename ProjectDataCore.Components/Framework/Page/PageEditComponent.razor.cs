using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageEditComponent
{
    protected ConcurrentDictionary<string, RenderFragment> ConfigurationNodes { get; set; } = new();
    public string? OpenConfigurationNode { get; set; } = null;
    public bool ShowConfigurationOptions { get; set; } = false;

    public async Task OnConfigureNodePushed(string name, RenderFragment fragment, bool dispose)
    {
        if (dispose)
        {
            _ = ConfigurationNodes.TryRemove(name, out _);
        }
        else
        {
            ConfigurationNodes[name] = fragment;
        }

        await InvokeAsync(StateHasChanged);
    }
}
