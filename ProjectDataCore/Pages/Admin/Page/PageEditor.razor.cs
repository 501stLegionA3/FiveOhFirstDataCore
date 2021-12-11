using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Page;
public partial class PageEditor
{
    [Inject]
    public 

    public class NewPageDataModel
    {
        public string Route { get; set; }
        public string Name { get; set; }
    }

    public NewPageDataModel NewPageData { get; set; } = new();

    private async Task OnNewPageSubmitAsync()
    {

    }
}
