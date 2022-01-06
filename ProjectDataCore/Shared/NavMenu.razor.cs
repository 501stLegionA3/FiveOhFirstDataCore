namespace ProjectDataCore.Shared;

public partial class NavMenu
{
    private List<Link> _modules = new List<Link>
    {
        new("Admin", "/admin", new List<Link>
        {
            new("Page Editor", "/admin/page/edit"),
            new("Roster Editor", "/admin/roster/edit", new List<Link>
            {
                new("Roster List", "/admin/roster/list", new List<Link>
                {
                    new("Test", "/admin/roster/list/test")
                }, true)
            }, true)
        })
    };

    [Inject] public NavigationManager _navManager { get; set; }


    private void Navigate(string href)
    {
        _navManager.NavigateTo(href, true);
    }
}

/// <summary>
/// Used to represent modules for the top navigation bar
/// </summary>
public class Link
{
    public Link(string displayName, string href, bool hasMainPage = false)
    {
        DisplayName = displayName;
        Href = href;
        HasMainPage = hasMainPage;
    }

    public Link(string displayName, string href, List<Link> subLinks, bool hasMainPage = false)
    {
        DisplayName = displayName;
        Href = href;
        SubLinks = subLinks;
        HasMainPage = hasMainPage;
    }

    public string DisplayName { get; set; }
    public string Href { get; set; }
    public List<Link>? SubLinks { get; set; }
    public bool HasMainPage { get; set; } = false;
}