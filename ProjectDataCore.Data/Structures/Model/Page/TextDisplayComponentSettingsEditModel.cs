using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;
public class TextDisplayComponentSettingsEditModel : CustomPageSettingsEditModel
{
    public string? RawContents { get; set; } = null;

    #region Editing Permissions
    public bool? PrivateEdit { get; set; } = null;
    public Optional<Guid?> EditPolicyKey { get; set; } = Optional.FromNoValue<Guid?>();
    #endregion
}
