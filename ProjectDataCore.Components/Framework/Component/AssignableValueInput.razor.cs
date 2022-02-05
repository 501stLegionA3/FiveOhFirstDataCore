using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Component;
public partial class AssignableValueInput
{
    [Parameter]
    public BaseAssignableConfiguration? ToEdit { get; set; } = null;
    [Parameter]
    public AssignableConfigurationValueEditModel EditModel { get; set; }
    [Parameter]
    public Action? SingleValueUpdate { get; set; }

    private void ValueChanged(Action updateValue)
    {
        updateValue.Invoke();

        if (SingleValueUpdate is not null)
            SingleValueUpdate.Invoke();
    }
}
