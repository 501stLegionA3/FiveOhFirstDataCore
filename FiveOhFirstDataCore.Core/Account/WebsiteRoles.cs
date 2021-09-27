using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Account
{
    public enum WebsiteRoles
    {
        [Description("Admin")]
        Admin,
        [Description("Manager")]
        Manager,
        [Description("MP")]
        MP,
        [Description("Trooper")]
        Trooper,
        [Description("RTO")]
        RTO,
        [Description("Medic")]
        Medic,
        [Description("ARC")]
        ARC,
        [Description("NCO")]
        NCO,
        [Description("Archived")]
        Archived
    }
}
