using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;
public partial class ParentFormCascader
{
    [Parameter]
    public LayoutComponentSettings ComponentSettings { get; set; }
    [Parameter]
    public RenderFragment? FormCap { get; set; }
    [Parameter]
    public List<DataCoreUser> SelectedUsers { get; set; } = new();

    [CascadingParameter(Name = "ParentForm")]
    public FormLayoutBase? ParentForm { get; set; }
}
