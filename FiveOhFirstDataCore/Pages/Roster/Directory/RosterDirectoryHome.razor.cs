using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Roster.Directory
{
    public partial class RosterDirectoryHome
    {
        public List<(string, string)> Urls { get; set; } = new() { ("/", "Home"), ("/roster", "Roster"), ("/roster/directory", "Directories") };
    }
}
