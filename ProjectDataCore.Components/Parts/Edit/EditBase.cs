using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit;

public class EditBase : CustomComponentBase
{
    [Parameter]
    public EditableComponentSettings? ComponentData { get; set; }


}
