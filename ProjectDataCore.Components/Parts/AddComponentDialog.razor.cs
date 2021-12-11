using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts;
public partial class AddComponentDialog
{
    [Inject]
    public IPageEditService PageEditService { get; set; }

    [Parameter]
    public Type[] AllowedAttributes { get; set; } = Array.Empty<Type>();

    [Parameter]
    public Guid? BasePage { get; set; }
    [Parameter]
    public Guid? ParentComponent { get; set; }

    private int ComponentTypeIndex { get; set; }
    private List<(Type, string)> AvalibleTypes { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            AvalibleTypes.Clear();

            var asm = Assembly.GetAssembly(GetType());
            var types = asm!.GetTypes();
            foreach (var t in types)
            {
                foreach (var attrType in AllowedAttributes)
                {
                    var attr = t.GetCustomAttributes(attrType)
                        .FirstOrDefault() as ComponentAttribute;

                    if (attr is not null && t.FullName is not null)
                    {
                        AvalibleTypes.Add((t, attr.Name));
                        break;
                    }
                }
            }

            StateHasChanged();
        }
    }

    private async Task OnAddComponentAsync()
    {
        var typ = AvalibleTypes[ComponentTypeIndex].Item1;

        if (BasePage is not null && ParentComponent is not null)
            throw new Exception($"Both {nameof(BasePage)} and {nameof(ParentComponent)} can not have a value.");

        if (BasePage is not null)
            await PageEditService.SetPageLayoutAsync(BasePage.Value, typ);
        else if (BasePage is not null)
            // TODO: Add component assigment for layout componenets.
            return;
        else
            throw new Exception($"Either {nameof(BasePage)} or {nameof(ParentComponent)} must have a value, but not both.");
    }
}
