using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Manager;
public partial class ManagerAccessSettings
{
    public List<(string, string)> Urls = new()
    {
        ("/", "Home"),
        ("/manager", "Manager Home"),
        ("/manager/access", "Access Settings")
    };

    #region View
    private enum ManagerAccessView
    {
        Sections,
        Policies
    }

    private ManagerAccessView Active { get; set; } = ManagerAccessView.Sections;

    private void OnTypeChange(ManagerAccessView option)
    {
        Active = option;
        StateHasChanged();
    }
    #endregion


}
