using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;
public class ButtonComponentSettingsEditModel
{
    public string? DisplayName { get; set; } = null;
    public ButtonComponentSettings.ButtonStyle? Style { get; set; } = null;
    public bool? InvokeSave { get; set; } = null;
    public bool? ResetForm { get; set; } = null;
}
