using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Render;
public class AssignableValueRenderer : DataObject<Guid>
{
    /// <summary>
    /// The base component that this renderer is attached to.
    /// </summary>
    public ParameterComponentSettingsBase ParameterComponentSettings { get; set; }
    /// <summary>
    /// The ID of the <see cref="ParameterComponentSettings"/>.
    /// </summary>
    public Guid ParemeterComponentSettingsId { get; set; }

    /// <summary>
    /// The display name for this <see cref="AssignableValueRenderer"/> object.
    /// </summary>
    public string DisplayName { get; set; } = "New Renderer";

    /// <summary>
    /// The name of the assignable value or static property to access.
    /// </summary>
    public string PropertyName { get; set; }
    /// <summary>
    /// If <b>true</b>, the <see cref="PropertyName"/> references a static property.
    /// </summary>
    public bool Static { get; set; }

    /// <summary>
    /// The converion objects for this assignable value.
    /// </summary>
    public List<AssignableValueConversion> Conversions { get; set; } = new();

    /// <summary>
    /// Gets the value pairs for this assignable value.
    /// </summary>
    /// <param name="user">The user object to retrieve values from.</param>
    /// <returns>A dictionary with string keys that can be matched to tokens in a string and
    /// values of <see cref="AssignableValueRenderData"/> that contain the value information
    /// for the configured conversion.</returns>
    public Dictionary<string, AssignableValueRenderData> GetValuePairs(DataCoreUser user)
    {
        BaseAssignableValue? assignable;
        if(Static)
        {
            assignable = user.GetStaticPropertyContainer(PropertyName);
        }
        else
        {
            assignable = user.GetAssignablePropertyContainer(PropertyName);
        }

        var res = new Dictionary<string, AssignableValueRenderData>();
        if (assignable is null)
            return res;

        foreach(var conv in Conversions)
        {
            var convRes = conv.FormatAssignable(assignable);

            var fullData = new AssignableValueRenderData(conv.ValueName, null, convRes.Item1);
            _ = res.TryAdd(fullData.Composite, fullData);

            int i = 1;
            foreach (string single in convRes.Item2)
            {
                var dataPart = new AssignableValueRenderData(conv.ValueName, i++, single);
                _ = res.TryAdd(dataPart.Composite, dataPart);
            }
        }

        return res;
    }
}
