using Newtonsoft.Json;

using System.Collections.Generic;

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
