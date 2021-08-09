using System.ComponentModel;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum PrefferedRole
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
