using ProjectDataCore.Data.Structures.Assignable.Render;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Page.Components.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Components.Display;

[DisplayComponent(typeof(DisplayComponentSettings), 
    Name = "Assignable Value Display",
    ShortName = "Value Display",
    IconPath = "svg/mat-icons/text_fields.svg"
)]
public partial class AssignableValueDisplayComponent : DisplayBase
{
    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

    #region Assignable Values
    private static readonly Type[] AllowedStaticTypes = new[]
    {
        // TODO add Guid support for assignable types
        // typeof(Guid),
        typeof(string),
        typeof(ulong)
    };

    private AssignableValueRenderer? AssignableValueRendererToEdit { get; set; }
    private AssignableValueConversion? AssignableValueConversionToEdit { get; set; }
    private BaseAssignableValue? ValueConversionContainer { get; set; }

    private void AddAssignableValueRenderer()
    {
        ComponentData?.AssignableValueRenderers.Add(new()
        {
            ParameterComponentSettings = ComponentData
        });

        EditComponent?.RequestStateHasChanged();
    }

    private void DeleteAssignableValueRenderer(AssignableValueRenderer renderer)
    {
        if(AssignableValueRendererToEdit == renderer)
        {
            AssignableValueRendererToEdit = null;
        }

        ComponentData?.AssignableValueRenderers.Remove(renderer);

        EditComponent?.RequestStateHasChanged();
    }

    private void EditAssignableValueRenderer(AssignableValueRenderer? renderer)
    {
        AssignableValueRendererToEdit = renderer;

        EditComponent?.RequestStateHasChanged();
    }

    private void AddAssignableValueConversion()
    {
        AssignableValueRendererToEdit?.Conversions.Add(new()
        {
            Renderer = AssignableValueRendererToEdit
        });

        EditComponent?.RequestStateHasChanged();
    }

    private void DeleteAssignableValueConversion(AssignableValueConversion conversion)
    {
        if (AssignableValueConversionToEdit == conversion)
        {
            AssignableValueConversionToEdit = null;
        }

        AssignableValueRendererToEdit?.Conversions.Remove(conversion);

        EditComponent?.RequestStateHasChanged();
    }

    private void EditAssignableValueConversion(AssignableValueConversion? conversion)
    {
        AssignableValueConversionToEdit = conversion;

        ReloadValueConversionContainer();

        EditComponent?.RequestStateHasChanged();
    }

    private void ReloadValueConversionContainer()
    {
        if (ActiveUser is not null
            && AssignableValueRendererToEdit is not null)
        {
            if (AssignableValueRendererToEdit.Static)
            {
                ValueConversionContainer = ActiveUser.GetStaticPropertyContainer(AssignableValueRendererToEdit.PropertyName);
            }
            else
            {
                ValueConversionContainer = ActiveUser.GetAssignablePropertyContainer(AssignableValueRendererToEdit.PropertyName);
            }
        }
    }

    private void DateTime_ConvertToTimeSpan_ValueChanged(bool x)
    {
        if (AssignableValueConversionToEdit is not null)
        {
            AssignableValueConversionToEdit.DateTime_ConvertToTimeSpan = x;

            EditComponent?.RequestStateHasChanged();
        }
    }

    private void DateTime_ManualTimeSpanComparision_ValueChanged(bool x)
    {
        if (AssignableValueConversionToEdit is not null)
        {
            AssignableValueConversionToEdit.DateTime_ManualTimeSpanComparision = x;

            EditComponent?.RequestStateHasChanged();
        }
    }
    #endregion
}
