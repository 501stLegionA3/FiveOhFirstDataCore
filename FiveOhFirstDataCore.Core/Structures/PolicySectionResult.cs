using FiveOhFirstDataCore.Core.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures;

public class PolicySectionResult : ResultBase
{
    private readonly PolicySection? _section;

    public PolicySectionResult(bool success, PolicySection? section, List<string>? errors = null) : base(success, errors)
        => _section = section;

    public bool GetResult([NotNullWhen(true)] out PolicySection section, [NotNullWhen(false)] out List<string> errors)
    {
        section = _section!;
        errors = Errors!;
        return Success;
    }
}
