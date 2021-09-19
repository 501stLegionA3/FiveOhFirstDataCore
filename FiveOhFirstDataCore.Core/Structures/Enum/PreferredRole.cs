using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures
{
    public enum PreferredRole
    {
        [Description("Trooper")]
        Trooper,
        [Description("RTO")]
        RTO,
        [Description("Medic")]
        Medic,
        [Description("Assault")]
        Assault,
        [Description("Support")]
        Support,
        [Description("Grenadier")]
        Grenadier,
        [Description("Marksman")]
        Marksman,
        [Description("Combat Engineer")]
        CE
    }
}
