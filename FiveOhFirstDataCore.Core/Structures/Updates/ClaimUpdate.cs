using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class ClaimUpdate
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";

        public ClaimUpdate() { }
        public ClaimUpdate(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
