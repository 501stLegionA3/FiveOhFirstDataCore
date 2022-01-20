using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;

// TODO Update to include more options on what to do when submitting.

/// <summary>
/// Holds information about buttons.
/// </summary>
public class ButtonComponentSettings : PageComponentSettingsBase
{
    public enum ButtonStyle
    {
        [Description("OP Danger")]
        OpDanger,
        [Description("OP Success")]
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
            ButtonStyle.OpDanger => "button-op-danger",
            ButtonStyle.OpSuccess => "button-op-success",
            _ => "",
        };
}
