using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

/// <summary>
/// Holds information about buttons.
/// </summary>
public class ButtonComponentSettings : PageComponentSettingsBase
{
    public enum ButtonStyle
    {
        OpDanger,
        OpSuccess
    }

    /// <summary>
    /// Will this button reset the parent form?
    /// </summary>
    public bool ResetForm { get; set; } = true;

    /// <summary>
    /// Will this button invoke a save?
    /// </summary>
    public bool InvokeSave { get; set; } = false;
    
    /// <summary>
    /// The style the buttons should use.
    /// </summary>
    public ButtonStyle Style { get; set; }

    public string GetColorClasses()
        => Style switch
        {
            ButtonStyle.OpDanger => "border border-op_danger hover:bg-op_danger hover:text-op_danger_t",
            ButtonStyle.OpSuccess => "border border-op_success hover:bg-op_success hover:text-op_success_t",
            _ => "",
        };
}
