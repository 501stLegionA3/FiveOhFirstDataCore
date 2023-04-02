using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures
{
    public enum Team
    {
        [Description("Alpha Team")]
        Alpha,
        [Description("Bravo Team")]
        Bravo
    }

    public enum Flight
    {
        [Description("Flight Lead")]
        Alpha,
        [Description("Section Lead")]
        Bravo,
        [Description("Third Pilot")]
        Charlie,
        [Description("Fourth Pilot")]
        Delta,
        [Description("Echo Pilot(s)")]
        Echo
    }
}
