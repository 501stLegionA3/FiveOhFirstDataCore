using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Policy;

public class PolicyClaimData
{
    public Guid Key { get; set; }
    public string Claim { get; set; }
    public string Value { get; set; }
}
