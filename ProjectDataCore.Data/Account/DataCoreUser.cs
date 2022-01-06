using Microsoft.AspNetCore.Identity;

using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreUser : IdentityUser<Guid>
{
    [Description("Display ID")]
    public int DisplayId { get; set; }
    [Description("Nickname")]
    public string NickName { get; set; }

    #region Assignable
    public List<BaseAssignableValue> AssignableValues { get; set; } = new();
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

    public string GetStaticProperty(string property, string? format = null)
    {
        var typ = GetType();
        var p = typ.GetProperty(property);
        if (p is not null)
        {
            var val = p.GetValue(this);

            var s = Convert.ToString(val);

            return string.Format(format ?? "{0}", s);
        }

        return string.Empty;
    }

    public string GetAssignableProperty(string property, string? format = null)
    {
        // TODO

        throw new NotImplementedException();
    }
}
