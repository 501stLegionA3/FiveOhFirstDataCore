using DSharpPlus.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Misc;

public partial class StaticDiscordChannelInput
{
    [Parameter]
    public ulong[] Item { get; set; } = new ulong[1];
    [Parameter]
    public bool MakeListNullOnEmpty { get; set; } = true;
    [Parameter]
    public IReadOnlyList<DiscordChannel> Source { get; set; } = Array.Empty<DiscordChannel>();

    public string NewValueFilter { get; set; } = "";
    public ulong NewValue { get; set; } = 0;

    public void AddValue()
    {
        if (NewValue > 0)
        {
            Item[0] = NewValue;
        }

        NewValue = 0;
    }
}
