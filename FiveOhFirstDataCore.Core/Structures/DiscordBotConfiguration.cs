using FiveOhFirstDataCore.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class DiscordBotConfiguration
    {
        public ulong HomeGuild { get; set; }
        public Dictionary<CShop, Dictionary<string, Dictionary<string, ulong[]>>> CShopRoleBindings { get; set; }
        public Dictionary<string, Dictionary<string, DiscordRoleDetails>> RoleBindings { get; set; }
    }
}
