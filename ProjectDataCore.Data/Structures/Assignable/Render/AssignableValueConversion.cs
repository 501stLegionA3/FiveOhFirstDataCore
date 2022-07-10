using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Util;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Render;
public class AssignableValueConversion : DataObject<Guid>
{
    public AssignableValueRenderer Renderer { get; set; }
    public Guid RendererId { get; set; }

    public string ValueName { get; set; } = "default";

    #region strings
    /// <summary>
    /// The format string to be applied to the string value.
    /// </summary>
    public string? String_FormatString { get; set; } = null;
    #endregion

    #region Integers/Doubles
    /// <summary>
    /// The string format to use for numerical values.
    /// </summary>
    public string? Numeric_FormatString { get; set; } = null;
    #endregion

    #region Date Time/Time Only/Date Only
    /// <summary>
    /// The string patten to use in a ToString conversion either for the <see cref="DateTime"/>/<see cref="TimeOnly"/>
    /// or the resulting <see cref="TimeSpan"/> from a conversion when <see cref="DateTime_ConvertToTimeSpan"/> is true.
    /// </summary>
    /// <remarks>
    /// This value can be a 
    /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings">Date Time Format String</a>
    /// or a <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings">Time Span Format String</a>.
    /// <br /><br />
    /// The following special values are also allowed:<br />
    /// short<br />
    /// long<br />
    /// <br />
    /// Short will convert the value with <see cref="DateTime.ToShortDateString"/> (or the respective method), 
    /// while long uses <see cref="DateTime.ToLongDateString"/> (or the respective method).
    /// </remarks>
    public string? DateTime_ToStringPattern { get; set; } = null;
    /// <summary>
    /// If the <see cref="DateTime"/> should be converted to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <remarks>
    /// For <see cref="TimeOnly"/> values, the <see cref="TimeOnly.ToTimeSpan"/> method will be used. For <see cref="DateOnly"/> values,
    /// the current time will always be used.
    /// </remarks>
    public bool DateTime_ConvertToTimeSpan { get; set; } = false;
    /// <summary>
    /// The <see cref="DateTime"/> that the saved value should be compared to. Leave as null to compare
    /// against <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public DateTime? DateTime_TimeSpanConversionCompareTo { get; set; } = null;
    #endregion

    #region Boolean
    /// <summary>
    /// The format string to change the boolean value to when
    /// the value is true.
    /// </summary>
    public string Bool_FormatOnTrue { get; set; } = "True";
    /// <summary>
    /// The format string to change the boolean value to when
    /// the value is false.
    /// </summary>
    public string Bool_FormatOnFalse { get; set; } = "False";
    #endregion

    #region Multiple Values
    /// <summary>
    /// The maximum ammount of items to Display. Set to 0 to display all items that are saved.
    /// </summary>
    public int Multi_MaxValues { get; set; } = 0;
    /// <summary>
    /// The separator to place between each value.
    /// </summary>
    public string Multi_Separator { get; set; } = "\n";
    #endregion

    public (string, List<string>) FormatAssignable(BaseAssignableValue valueContainer)
    {
        List<string> formattedValues = valueContainer switch
        {
            IAssignableValue<string> sv => FormatStrings(sv.SetValue),
            IAssignableValue<int> iv => FormatNumbers(iv.SetValue),
            IAssignableValue<double> dv => FormatNumbers(dv.SetValue),
            IAssignableValue<DateTime> dt => FormatDateTimes(dt.SetValue),
            IAssignableValue<DateOnly> donly => FormatDateOnlys(donly.SetValue),
            IAssignableValue<TimeOnly> tonly => FormatTimeOnlys(tonly.SetValue),
            IAssignableValue<bool> bv => FormatBooleans(bv.SetValue),
            _ => new()
        };

        List<string> publishSet = formattedValues;
        if(Multi_MaxValues > 0)
            publishSet = formattedValues.Take(Multi_MaxValues).ToList();

        string final = string.Format(Multi_Separator, publishSet);
        return (final, formattedValues);
    }

    private List<string> FormatStrings(List<string> values)
    {
        List<string> res = new();
        foreach(var item in values)
        {
            if (item is not null)
            {
                if (String_FormatString is not null)
                    res.Add(string.Format(String_FormatString, item));
                else
                    res.Add(item);
            }
        }
        return res;
    }

    private List<string> FormatNumbers(List<dynamic?> numbers)
    {
        List<string> res = new();
        foreach (var item in numbers)
        {
            if (item is not null)
            {
                res.Add(string.Format(Numeric_FormatString, item));
            }
        }
        return res;
    }

    private List<string> FormatNumbers(List<int> ints)
        => FormatNumbers(ints.ToList(x => (dynamic?)x));

    private List<string> FormatNumbers(List<double> doubles)
        => FormatNumbers(doubles.ToList(x => (dynamic?)x));

    private List<string> FormatDateTimes(List<DateTime> dateTimes)
    {
        if(DateTime_ConvertToTimeSpan)
        {
            var compare = DateTime_TimeSpanConversionCompareTo ?? DateTime.UtcNow;
            return FormatTimeSpans(dateTimes.ToList(x => x - compare));
        }

        return DateTime_ToStringPattern switch
        {
            "long" => dateTimes.ToList(x => x.ToLongDateString()),
            "short" => dateTimes.ToList(x => x.ToShortDateString()),
            _ => dateTimes.ToList(x => x.ToString(DateTime_ToStringPattern)),
        };
    }

    private List<string> FormatDateOnlys(List<DateOnly> dateOnlys)
    {
        var time = TimeOnly.FromDateTime(DateTime.UtcNow);

        return FormatDateTimes(dateOnlys.ToList(x => x.ToDateTime(time)));
    }

    private List<string> FormatTimeSpans(List<TimeSpan> timeSpans)
        => timeSpans.ToList(x => x.ToString(DateTime_ToStringPattern));

    private List<string> FormatTimeOnlys(List<TimeOnly> timeOnlys)
    {
        if(DateTime_ConvertToTimeSpan)
        {
            return FormatTimeSpans(timeOnlys.ToList(x => x.ToTimeSpan()));
        }

        return DateTime_ToStringPattern switch
        {
            "long" => timeOnlys.ToList(x => x.ToLongTimeString()),
            "short" => timeOnlys.ToList(x => x.ToShortTimeString()),
            _ => timeOnlys.ToList(x => x.ToString(DateTime_ToStringPattern)),
        };
    }

    private List<string> FormatBooleans(List<bool> booleans)
    {
        List<string> res = new();
        foreach (var item in booleans)
        {
            res.Add(item ? Bool_FormatOnTrue : Bool_FormatOnFalse);
        }
        return res;
    }
}
