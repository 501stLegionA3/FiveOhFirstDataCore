using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts;

public abstract class ParameterBase : CustomComponentBase
{
    protected abstract Task RemoveCurrentDisplay();

    #region Edit Methods
    protected abstract void StartEdit();
    protected abstract void CancelEdit();
    protected abstract Task SaveEdit();
    #endregion

    #region Parameter Scope
    public string DisplayValue { get; set; }
    protected abstract Task LoadDisplayValueAsync();

    protected abstract void LoadStaticProperty();

    protected abstract Task LoadDynamicPropertyAsync();
    #endregion
}
