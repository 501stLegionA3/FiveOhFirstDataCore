using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Page;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

public partial class BasicFormPart : LayoutBase
{
#pragma warning disable CS8618 // Injections are non-nullable.
    [Inject]
    public IRoutingService RoutingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    [Parameter]
    public LayoutComponentSettings ComponentSettings { get; set; }
    [Parameter]
    public RenderFragment? FormCap { get; set; }

    private Type[] AllowedAttributes { get; } = new Type[]
    {
        typeof(LayoutComponentAttribute),
        typeof(EditableComponentAttribute),
        typeof(DisplayComponentAttribute),
        typeof(RosterComponentAttribute)
    };

    /// <summary>
    /// The type of base layout component to display.
    /// </summary>
    private Type? ComponentType { get; set; }
    /// <summary>
    /// The parameters for the component.
    /// </summary>
    private Dictionary<string, object> ComponentParams { get; set; } = new()
    {
        { "ComponentData", null }
    };

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var cfg = ComponentSettings?.ChildComponents.FirstOrDefault();
        if (cfg is not null)
        {
            try
            {
                // Save the items to the array.
                ComponentType = RoutingService.GetComponentType(cfg.QualifiedTypeName);
                ComponentParams = new()
                {
                    { "ComponentData", cfg }
                };
            }
            catch (MissingComponentException)
            {
                ComponentType = typeof(MissingComponentExceptionNotice);
                ComponentParams = new()
                {
                    { "ComponentData", cfg }
                };
            }
        }
    }
}
