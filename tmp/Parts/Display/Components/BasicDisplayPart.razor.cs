using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Display.Components;

[DisplayComponent(Name = "Basic Single Value")]
public partial class BasicDisplayPart : DisplayBase
{
	protected Type[] AllowedStaticTypes { get; set; } = new Type[]
	{
		typeof(string)
	};
}
