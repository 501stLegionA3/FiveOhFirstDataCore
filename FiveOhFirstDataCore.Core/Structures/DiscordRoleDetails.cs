using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class DiscordRoleDetails
    {
        public string Key { get; set; }
        [JsonProperty("RoleGrants")]
        public List<ulong> RoleGrants { get; set; } = new();
        [JsonProperty("RoleReplaces")]
        public List<ulong> RoleReplaces { get; set; } = new();
    }
}
