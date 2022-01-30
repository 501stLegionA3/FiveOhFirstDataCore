using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts;

public abstract class ParameterBase : CustomComponentBase
{
    protected class ParameterEditModel
    {
        public Guid Key { get; init; }
        public string? Label { get; set; }
        public string Property { get; set; }
        public bool StaticProperty { get; set; }
        public string? FormatString { get; set; }
        public List<RosterDisplaySettings>? AllowedDisplays { get; set; }

        public ParameterEditModel(ParameterComponentSettingsBase settings)
        {
            Key = settings.Key;
            Label = settings.Label;
            Property = settings.PropertyToEdit;
            StaticProperty = settings.StaticProperty;
            FormatString = settings.FormatString;
        }
    }

    protected ParameterEditModel? EditModel { get; set; }

    protected abstract Task RemoveCurrentDisplay();

    #region Edit Methods
    protected abstract void StartEdit();
    protected abstract void CancelEdit();
    protected abstract Task SaveEdit();
    #endregion

    #region Parameter Scope
    public string DisplayValue { get; set; }
    protected abstract void LoadParameterValue();

    protected abstract void LoadStaticProperty();

    protected abstract void LoadDynamicProperty();
    #endregion

    #region User Scope
    public Guid? UserScopeSelection { get; set; }
    public void OnUserScopeChanged(Guid? userScope)
    {
        UserScopeSelection = userScope;
    }
    #endregion
}
