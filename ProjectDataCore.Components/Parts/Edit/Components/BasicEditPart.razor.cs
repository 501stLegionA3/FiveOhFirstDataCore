using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit.Components;

[EditableComponent(Name = "Basic Single Value")]
public partial class BasicEditPart : EditBase
{
	protected Type[] AllowedStaticTypes { get; set; } = new Type[]
	{
		typeof(string)
	};
}
