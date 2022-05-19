using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Render;
public class AssignableValueRenderer : DataObject<Guid>
{
    public string PropertyName { get; set; }
    public bool Static { get; set; }

    public List<AssignableValueConversion> Conversions { get; set; } = new();

    public Dictionary<string, AssignableValueRenderData> GetValuePairs(DataCoreUser user)
    {
        BaseAssignableValue? assignable;
        if(Static)
        {
            var obj = user.GetStaticPropertyObject(PropertyName);
            assignable = obj switch
            {
                int a => new IntegerAssignableValue()
                {
                    SetValue = new()
                    {
                        a
                    }
                },
                double d => new DoubleAssignableValue()
                {
                    SetValue = new()
                    {
                        d
                    }
                },

                string s => new StringAssignableValue()
                {
                    SetValue = new()
                    {
                        s
                    }
                },

                bool b => new BooleanAssignableValue()
                {
                    SetValue = new()
                    {
                        b
                    }
                },

                DateTime dt => new DateTimeAssignableValue()
                {
                    SetValue = new()
                    {
                        dt
                    }
                },
                TimeOnly tonly => new TimeOnlyAssignableValue()
                {
                    SetValue = new()
                    {
                        tonly
                    }
                },
                DateOnly donly => new DateOnlyAssignableValue()
                {
                    SetValue = new()
                    {
                        donly
                    }
                },

                _ => null,
            };
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
