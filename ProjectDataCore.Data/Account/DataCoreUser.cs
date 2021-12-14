using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreUser : IdentityUser<int>
{
    [Description("Display ID")]
    public int DisplayId { get; set; }
    [Description("Nickname")]
    public string NickName { get; set; }

    #region Assignable

    #endregion

    #region Roster
    public List<RosterSlot> RosterSlots { get; set; } = new();
    #endregion

    [Description("Discord ID")]
    public ulong? DiscordId { get; set; }
    [Description("Steam Account")]
    public string? SteamLink { get; set; }
    [Description("Access Code")]
    public string? AccessCode { get; set; }
}
