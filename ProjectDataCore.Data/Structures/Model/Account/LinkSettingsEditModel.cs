using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Account;
public class LinkSettingsEditModel
{
    public bool? RequireDiscordLink { get; set; } = null;
    public bool? RequireSteamLink { get; set; } = null;
}
