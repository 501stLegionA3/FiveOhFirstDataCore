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

    #region strings
    /// <summary>
    /// The alignment value for the string.
    /// </summary>
    public int String_Alignment { get; set; }
    /// <summary>
    /// The format string to be applied to the string value.
    /// </summary>
    public string? String_FormatString { get; set; } = null;
    #endregion

    #region Integers/Doubles
    public string? Numeric_FormatString { get; set; }
    #endregion

    #region Date Time/Time Only/Date Only
    public string? DateTime_ToStringPattern { get; set; }
    public bool DateTime_ConvertToTimeSpan { get; set; }
    public DateTimeToTimeSpanConversionOptions DateTime_ConversionOptions { get; set; }
    public DateTime? DateTime_TimeSpanConversionCompareTo { get; set; }
    #endregion

    #region Boolean
    public bool Bool_UseStringLiteral { get; set; }
    public string? Bool_FormatString { get; set; }
    #endregion
}

public enum DateTimeToTimeSpanConversionOptions
{
    [Description("Seconds")]
    Seconds,
    [Description("Minutes")]
    Minutes,
    [Description("Hours")]
    Hours,
    [Description("Days")]
    Days,
    [Description("Months")]
    Months,
    [Description("Years")]
    Years
}
