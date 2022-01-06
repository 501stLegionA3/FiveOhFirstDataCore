namespace ProjectDataCore.Shared;

public partial class NavMenu
{
    [Inject]
    public NavigationManager _navManager {get; set;}

    private List<LinkGroup> _modules = new List<LinkGroup>()
    {
        new LinkGroup("Admin", "/admin", new List<Link>() {new Link("Page Editor", "/admin/page/edit"), new Link("Roster Editor", "/admin/roster/edit")}),
    };
    
    

    private void Navigate(string href)
    {
        _navManager.NavigateTo(href, true);
    }
}

class LinkGroup
{
    public LinkGroup(string displayName, string href, List<Link> links)
    {
        DisplayName = displayName;
        HREF = href;
        Links = links;
    }

    public String DisplayName { get; set; }
    public List<Link> Links { get; set; }
    public String HREF { get; set; }
}

class Link
{
    public Link(string display, string href)
    {
        Display = display;
        HREF = href;
    }

    public String Display { get; set; }
    public String HREF { get; set; }
    
    
}