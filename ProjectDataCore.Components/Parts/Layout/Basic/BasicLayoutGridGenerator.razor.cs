using ProjectDataCore.Data.Services.Routing;

namespace ProjectDataCore.Components.Parts.Layout.Basic;

public partial class BasicLayoutGridGenerator : LayoutBase
{
#pragma warning disable CS8618 // Injections are non-nullable.
    [Inject]
    public IRoutingService RoutingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public int Width { get; set; }
    [Parameter]
    public int Height { get; set; }
    [Parameter]
    public LayoutComponentSettings ComponentSettings { get; set; }

    private Type[] AllowedAttributes { get; } = new Type[]
    {
        typeof(LayoutComponentAttribute),
        typeof(EditableComponentAttribute),
        typeof(DisplayComponentAttribute)
    };

    private PageComponentSettingsBase[] ConfiguredSettings { get; set; }
    private Type[] ConfiguredTypes { get; set; }
    private Dictionary<string, object>[] ConfiguredParameters { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var size = Width * Height;
        ConfiguredSettings = new PageComponentSettingsBase[size];
        ConfiguredTypes = new Type[size];
        ConfiguredParameters = new Dictionary<string, object>[size];
        if (ComponentSettings is not null)
        {
            foreach (var item in ComponentSettings.ChildComponents)
            {
                try
                {
                    // Save the items to the array.
                    ConfiguredSettings[item.Order] = item;
                    ConfiguredTypes[item.Order] = RoutingService.GetComponentType(item.QualifiedTypeName);
                    ConfiguredParameters[item.Order] = new()
                    {
                        { "ComponentData", item }
                    };
                }
                catch (IndexOutOfRangeException)
                {
                    // Ignore index errors
                    // They should not happen.
                    continue;
                    // Other errors indicate something horrid went wrong
                }
            }
        }
    }
}
