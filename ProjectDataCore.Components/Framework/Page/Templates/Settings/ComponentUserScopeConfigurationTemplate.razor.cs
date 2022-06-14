using ProjectDataCore.Components.Framework.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Templates.Settings;
public partial class ComponentUserScopeConfigurationTemplate
{
#pragma warning disable CS8618 // Required parameters are not null.
	[Parameter]
	[EditorRequired]
	public PageComponentSettingsBase Settings { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


}
