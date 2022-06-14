using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Selectors.Settings;
public partial class AuthorizationPolicySelector
{
#pragma warning disable CS8618 // Required settings are never null. Injections are never null.
	[Inject]
	public IPolicyService PolicyService { get; set; }

	[Parameter, EditorRequired]
	public LayoutNode Settings { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


}
