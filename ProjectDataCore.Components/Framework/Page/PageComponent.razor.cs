namespace ProjectDataCore.Components.Framework.Page;
public partial class PageComponent
{
    [CascadingParameter(Name = "PageEdit")]
    public bool IsEditing { get; set; }

    [Parameter]
    public RenderFragment? Editing { get; set; }
    [Parameter]
    public RenderFragment Publish { get; set; }
    [Parameter]
    public RenderFragment? Configure { get; set; }
    [Parameter]
    public string Name { get; set; }
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Publish is null)
            throw new ArgumentException("There must be at least a publish section to a Page Component.", nameof(PageComponent));
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("The name must have a non-blank value.", nameof(Name));
    }
}