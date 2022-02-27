using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;

using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components;
public class TextDisplayComponentSettings : PageComponentSettingsBase
{
    public string RawContents { get; set; } = "";

    #region Editing Permissions
    public bool PrivateEdit { get; set; } = false;
    /// <summary>
    /// The Authorization Policy for when <see cref="PrivateEdit"/> is <i><b>true</b></i>.
    /// </summary>
    public DynamicAuthorizationPolicy? EditPolicy { get; set; }
    /// <summary>
    /// The key for the <see cref="EditPolicy"/>
    /// </summary>
    public Guid? EditPolicyKey { get; set; }
    #endregion

    #region Non-database Values
    public MarkupString Display => new(RawContents);
    #endregion
}
