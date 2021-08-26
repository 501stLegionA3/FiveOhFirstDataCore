using FiveOhFirstDataCore.Core.Account.Detail;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperReportPage
    {
        private TrooperReport Report { get; set; } = new();
        public string FirstReportMessage { get; set; } = "";

        private async Task OnSubmit()
        {

        }

        private async Task OnCancel()
        {

        }
    }
}
