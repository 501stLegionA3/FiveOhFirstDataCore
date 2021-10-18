using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Discord;

public class DiscordPostActionConfiguration
{
    public DiscordAction Action { get; set; }
    public ulong DiscordChannel { get; set; }
    public string? RawMessage { get; set; }
}
