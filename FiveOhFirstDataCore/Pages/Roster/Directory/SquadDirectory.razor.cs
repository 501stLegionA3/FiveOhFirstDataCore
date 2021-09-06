using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Pages.Roster.Directory
{
    public partial class SquadDirectory
    {
        public List<(string, string)> Urls { get; set; } = new()
        {
            ("/", "Home"),
            ("/roster", "Roster"),
            ("/roster/directory", "Directories"),
            ("/roster/directory/squad", "Squad Directory")
        };
        private string Search { get; set; } = "";
        private string SearchDummy { get; set; } = "";

        private void SearchUpdate(ChangeEventArgs e)
        {
            Search = (string?)e?.Value ?? "";
        }
    }
}
