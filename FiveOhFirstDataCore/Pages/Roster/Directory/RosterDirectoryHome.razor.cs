namespace FiveOhFirstDataCore.Pages.Roster.Directory
{
    public partial class RosterDirectoryHome
    {
        public List<(string, string)> Urls { get; set; } = new() { ("/", "Home"), ("/roster", "Roster"), ("/roster/directory", "Directories") };
    }
}
