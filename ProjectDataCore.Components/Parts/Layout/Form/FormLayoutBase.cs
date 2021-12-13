using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

public class FormLayoutBase : LayoutBase, IDisposable
{
    private bool registeredScope = false;

    protected void RegisterScope(ref DataCoreUser user)
    {
        if (ComponentData is not null)
        {
            registeredScope = true;
            ScopedUserService.LoadUserScope(ComponentData.Key, ref user);
        }
    }

    public void Dispose()
    {
        if (registeredScope && ComponentData is not null)
            ScopedUserService.UnloadUserScope(ComponentData.Key);
    }
}
