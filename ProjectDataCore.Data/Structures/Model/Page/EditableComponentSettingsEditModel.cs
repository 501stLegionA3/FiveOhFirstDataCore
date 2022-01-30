using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;

public class EditableComponentSettingsEditModel : ParameterComponentSettingsEditModel
{
	/// <summary>
	/// The allowed displays this component can edit. See <see cref="EditableComponentSettings.EditableDisplays"/>
	/// </summary>
	public List<RosterDisplaySettings>? EditableDisplays { get; set; }
}
