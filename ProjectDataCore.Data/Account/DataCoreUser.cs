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
    public List<BaseAssignableValue> AssignableValues { get; set; } = new();

    public List<RosterSlot> RosterSlots { get; set; } = new();

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
        var data = AssignableValues
            .Where(x => x.AssignableConfiguration.NormalizedPropertyName == property)
            .FirstOrDefault();

        if (data is null)
            return string.Empty;

        if (data.AssignableConfiguration.AllowMultiple)
        {
            var vals = data.GetValues();
            List<string> parts = new();
            foreach (var v in vals)
                if (v is not null)
                    parts.Add(string.Format(format ?? "{0}", v));

            return string.Join(", ", parts);
        }
        else
        {
            var val = data.GetValue();
            if(val is not null)
                return string.Format(format ?? "{0}", val);

            return "";
        }
    }
}
