using ProjectDataCore.Data.Structures.Page.Components.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Components.Display;

[DisplayComponent(typeof(DisplayComponentSettings), 
    Name = "Assignable Value Display", 
    IconPath = "~/svg/mat-icons/text_fields.svg"
)]
public partial class AssignableValueDisplayComponent : DisplayBase
{

}
