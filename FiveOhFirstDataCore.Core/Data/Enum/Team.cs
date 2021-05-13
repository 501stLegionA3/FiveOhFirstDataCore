using System.ComponentModel;

namespace FiveOhFirstDataCore.Core.Data
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
        [Description("Alpha Flight")]
        Alpha,
        [Description("Bravo Flight")]
        Bravo,
        [Description("Charlie Flight")]
        Charlie,
        [Description("Delta Flight")]
        Delta
    }
}
