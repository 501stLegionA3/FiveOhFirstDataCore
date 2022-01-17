using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts;
public partial class AddComponentDialog
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "RefreshRequest")]
    public Func<Task>? CallRefreshRequest { get; set; }

    /// <summary>
    /// The allowed attributes that a type of componenet can be selected from.
    /// </summary>
    [Parameter]
    public Type[] AllowedAttributes { get; set; } = Array.Empty<Type>();

    /// <summary>
    /// The base page for this component.
    /// </summary>
    /// <remarks>
    /// There must only be a <see cref="BasePage"/> OR a <see cref="ParentComponent"/>, not both and not none.
    /// </remarks>
    [Parameter]
    public Guid? BasePage { get; set; }
    /// <summary>
    /// The parent component for this component.
    /// </summary>
    /// <remarks>
    /// There must only be a <see cref="BasePage"/> OR a <see cref="ParentComponent"/>, not both and not none.
    /// </remarks>
    [Parameter]
    public Guid? ParentComponent { get; set; }
    /// <summary>
    /// A part of the parent component tracker,
    /// the position value determines where in a 
    /// parent component the new component is placed.
    /// </summary>
    [Parameter]
    public int Position { get; set; } = 0;

    private int LayoutComponentTypeIndex { get; set; }
    private int DisplayComponentTypeIndex { get; set; }
    private int EditableComponentTypeIndex { get; set; }
    /// <summary>
    /// The types avalbile to select from when adding new layout components.
    /// </summary>
    private List<(Type, string)> LayoutTypes { get; set; } = new();
    /// <summary>
    /// The types avalbile to select from when adding new display components.
    /// </summary>
    private List<(Type, string)> DisplayTypes { get; set; } = new();
    /// <summary>
    /// The types avalbile to select from when adding new editable components.
    /// </summary>
    private List<(Type, string)> EditableTypes { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            LayoutTypes.Clear();
            DisplayTypes.Clear();
            EditableTypes.Clear();
            // On load, get the current assembly ...
            var asm = Assembly.GetAssembly(GetType());
            // ... and the types from it ...
            var types = asm!.GetTypes();
            foreach (var t in types)
            {
                // ... then for each type, and each allowed attribute ...
                foreach (var attrType in AllowedAttributes)
                {
                    // ... check to see if the type has the attribute ...
                    var attr = t.GetCustomAttributes(attrType)
                        .FirstOrDefault() as ComponentAttribute;
                    // ... and add it to the avalible types list if it does ...
                    if (attr is not null && t.FullName is not null)
                    {
                        switch(attr)
                        {
                            case LayoutComponentAttribute:
                                LayoutTypes.Add((t, attr.Name));
                                break;
                            case DisplayComponentAttribute:
                            // A roster is a type of display.
                            case RosterComponentAttribute:
                                DisplayTypes.Add((t, attr.Name));
                                break;
                            case EditableComponentAttribute:
                                EditableTypes.Add((t, attr.Name));
                                break;
                        }

                        break;
                    }
                }
            }
            // ... then refresh the page.
            StateHasChanged();
        }
    }

    private async Task OnAddDisplayComponentAsync()
    {
        // On the add component action, get the item to add ...
        var typ = DisplayTypes[DisplayComponentTypeIndex].Item1;
        await OnAddComponentAsync(typ);
    }

    private async Task OnAddLayoutComponentAsync()
    {
        // On the add component action, get the item to add ...
        var typ = LayoutTypes[LayoutComponentTypeIndex].Item1;
        await OnAddComponentAsync(typ);
    }

    private async Task OnAddEditableComponentAsync()
    {
        // On the add component action, get the item to add ...
        var typ = EditableTypes[EditableComponentTypeIndex].Item1;
        await OnAddComponentAsync(typ);
    }

    private async Task OnAddComponentAsync(Type typ)
    {
        // TODO: Error displays for the Page Edit Service Methods.

        // ... ensure there is not two parent values ...
        if (BasePage is not null && ParentComponent is not null)
            throw new Exception($"Both {nameof(BasePage)} and {nameof(ParentComponent)} can not have a value.");
        // ... if it is a page requesting this, run the page method ...
        if (BasePage is not null)
        {
            await PageEditService.SetPageLayoutAsync(BasePage.Value, typ);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
        else if (ParentComponent is not null)
        {
            // ... otherwise run the layout method ...
            await PageEditService.SetLayoutChildAsync(ParentComponent.Value, typ, Position);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
        else
            // ... otherwise throw an exception if there are not parents found.
            throw new Exception($"Either {nameof(BasePage)} or {nameof(ParentComponent)} must have a value, but not both.");
    }
}
