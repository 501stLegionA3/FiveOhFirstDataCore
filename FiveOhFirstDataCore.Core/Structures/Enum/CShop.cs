using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures
{
    // DO NOT CHANGE NAMES
    // They are needed for permissions purposes.
    // If a name change is needed, a claim update will have to be completed for that name,
    // so members still have correct permissions.

    [Flags]
    public enum CShop : long
    {
        // 0 << 0
        None = 0x0000000000,
        // 1 << 1
        [Description("Roster Staff")]
        RosterStaff = 0x0000000001,
        // 1 << 2
        [Description("Document Maintenance Committee")]
        DocMainCom = 0x0000000002,
        // 1 << 3
        [Description("Recruiting Staff")]
        RecruitingStaff = 0x0000000004,
        // 1 << 4
        [Description("Returning Member Staff")]
        ReturningMemberStaff = 0x0000000008,
        // 1 << 5
        [Description("Medals Staff")]
        MedalsStaff = 0x0000000010,
        // 1 << 6
        [Description("Campaign Management")]
        CampaignManagement = 0x0000000020,
        // 1 << 7
        [Description("Event Management")]
        EventManagement = 0x0000000040,
        // 1 << 8
        [Description("Logistics Team")]
        Logistics = 0x0000000080,
        // 1 << 9
        [Description("TeamSpeak Administration")]
        // 1 << 10
        TeamSpeakAdmin = 0x0000000100,
        [Description("Holosite Support")]
        HolositeSupport = 0x0000000200,
        // 1 << 11
        [Description("Discord Management")]
        // 1 << 12
        DiscordManagement = 0x0000000400,
        [Description("Tech Support")]
        TechSupport = 0x0000000800,
        // 1 << 13
        [Description("Basic Combat Training Staff")]
        BCTStaff = 0x0000001000,
        // 1 << 14
        [Description("Private Training Instructor")]
        PrivateTrainingInstructor = 0x0000002000,
        // 1 << 15
        [Description("Upsilon Training Corps")]
        UTCStaff = 0x0000004000,
        // 1 << 16
        [Description("Qualification/MOS Training Staff")]
        QualTrainingStaff = 0x0000008000,
        // 1 << 17
        [Description("Server Management")]
        ServerManagement = 0x0000010000,
        // 1 << 18
        [Description("Auxiliary Mod Team")]
        AuxModTeam = 0x0000020000,
        // 1 << 19
        [Description("Public Affairs")]
        PublicAffairs = 0x0000040000,
        // 1 << 20
        [Description("Media/Streaming Outreach")]
        MediaOutreach = 0x0000080000,
        // 1 << 21
        [Description("News Team")]
        NewsTeam = 0x0000100000,
        // 1 << 22
        [Description("Big Brother")]
        BigBrother = 0x0000200000,
        // 1 << 23
        [Description("Department Lead")]
        DepartmentLead = 0x0000400000,
    }

    public static class CShopExtensions
    {
        public static CShop? GetCShop(this string value)
        {
            foreach (CShop enumValue in Enum.GetValues(typeof(CShop)))
            {
                if (enumValue.AsFull() == value)
                    return enumValue;
            }

            return null;
        }
    }
}
