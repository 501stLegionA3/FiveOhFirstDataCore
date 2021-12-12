namespace ProjectDataCore.Components.Parts.Layout;

public class LayoutBase : ComponentBase
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; }

    [Parameter]
    public LayoutComponentSettings? ComponentData { get; set; }
}
