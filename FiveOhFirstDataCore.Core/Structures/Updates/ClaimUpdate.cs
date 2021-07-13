using FiveOhFirstDataCore.Core.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class ClaimUpdate : UpdateBase
    {
        public List<ClaimUpdateData> Additions { get; set; } = new();
        public List<ClaimUpdateData> Removals { get; set; } = new();
        public bool AutomaticChange { get; set; } = true;

        public Trooper ChangedBy { get; set; }
        public int? ChangedById { get; set; }
    }

    public class ClaimUpdateData
    {
        public Guid UpdateKey { get; set; }

        public string Key { get; set; } = "";
        public string Value { get; set; } = "";

        public ClaimUpdateData() { }
        public ClaimUpdateData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
