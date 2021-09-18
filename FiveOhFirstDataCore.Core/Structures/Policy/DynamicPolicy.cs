using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Policy;

public class DynamicPolicy
{
    public string PolicyName { get; set; }

    public List<string> RequiredRoles { get; set; } = new();
    public List<PolicyClaimData> RequiredClaims { get; set; } = new();

    public string EditableByPolicyName { get; set; }
    public DynamicPolicy EditableByPolicy { get; set; }

    public List<PolicySection> PolicySections { get; set; } = new();
    public List<DynamicPolicy> CanEditPolicies { get; set; } = new();
}
