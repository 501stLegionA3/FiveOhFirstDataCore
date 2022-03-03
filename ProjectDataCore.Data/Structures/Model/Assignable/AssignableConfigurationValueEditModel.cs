using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Assignable;

public class AssignableConfigurationValueEditModel
{
    public DateTime DateValue { get; set; }
    public TimeSpan TimeValue { get; set; }
    public string NewOptionTime
    {
        get
        {
            return TimeValue.ToString();
        }

        set
        {
            if (TimeSpan.TryParse(value, out var t))
                TimeValue = t;
            else
                TimeValue = TimeSpan.Zero;
        }
    }
    public int IntValue { get; set; }
    public double DoubleValue { get; set; }
    public string StringValue { get; set; } = "";

    public AssignableConfigurationValueEditModel() { }

    public AssignableConfigurationValueEditModel(BaseAssignableValue value, int valueIndex = 0)
    {
        switch (value)
        {
            case IAssignableValue<DateTime> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                DateValue = DateOnly.FromDateTime(c.SetValue[valueIndex]).ToDateTime(TimeOnly.MinValue);
                TimeValue = TimeOnly.FromDateTime(c.SetValue[valueIndex]).ToTimeSpan();
                break;
            case IAssignableValue<DateOnly> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                DateValue = c.SetValue[valueIndex].ToDateTime(TimeOnly.MinValue);
                break;
            case IAssignableValue<TimeOnly> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                TimeValue = c.SetValue[valueIndex].ToTimeSpan();
                break;

            case IAssignableValue<int> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                IntValue = c.SetValue[valueIndex];
                break;
            case IAssignableValue<double> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                DoubleValue = c.SetValue[valueIndex];
                break;

            case IAssignableValue<string> c:
                if ((c.SetValue.Count - 1) < valueIndex)
                    break;

                StringValue = c.SetValue[valueIndex];
                break;
        }
    }

    public string GetValueAsString(BaseAssignableConfiguration value) 
        => value switch
        {
            IAssignableConfiguration<DateTime> => (DateValue + TimeValue).ToString(),
            IAssignableConfiguration<DateOnly> => DateValue.ToString(),
            IAssignableConfiguration<TimeOnly> => TimeValue.ToString(),
            IAssignableConfiguration<int> => IntValue.ToString(),
            IAssignableConfiguration<double> => DoubleValue.ToString(),
            IAssignableConfiguration<string> => StringValue,
            _ => string.Empty,
        };
}
