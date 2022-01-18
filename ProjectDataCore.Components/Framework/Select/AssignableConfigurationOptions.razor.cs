using ProjectDataCore.Data.Structures.Assignable.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Select;
public partial class AssignableConfigurationOptions
{
    [Parameter]
    public BaseAssignableConfiguration Config { get; set; }
    public List<string> ItemList { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ItemList = ((IAssignableConfiguration)Config).GetDisplayValues();
    }
}
