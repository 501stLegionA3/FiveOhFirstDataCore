using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Policy;

public class PolicySection
{
    public string SectionName { get; set; }
    public string PolicyName { get; set; }
    public DynamicPolicy Policy { get; set; }
}
