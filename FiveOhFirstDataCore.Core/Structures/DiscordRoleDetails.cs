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
    //[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DiscordRoleDetails
    {
        [JsonProperty("RoleGrants")]
        public ulong[] RoleGrants { get; set; }
        [JsonProperty("RoleReplaces")]
        public ulong[] RoleReplaces { get; set; }
        public DiscordRoleDetails()
        {
            RoleGrants = Array.Empty<ulong>();
            RoleReplaces = Array.Empty<ulong>();
        }
    }
}
