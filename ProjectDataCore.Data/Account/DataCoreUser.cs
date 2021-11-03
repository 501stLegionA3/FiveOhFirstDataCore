using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreUser : IdentityUser<int>
{
    public int DisplayId { get; set; }
    public string NickName { get; set; }

    public ulong? DiscordId { get; set; }
    public string? SteamLink { get; set; }
    public string? AccessCode { get; set; }
}
