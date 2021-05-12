using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data
{
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
        [Description("Recruting Staff")]
        RecruitingStaff = 0x0000000004,
        // 1 << 4
        [Description("Returing Member Staff")]
        ReturningMemberStaff = 0x0000000008,
        // 1 << 5
        [Description("Campaign Management")]
        CampaignManagement = 0x0000000010,
        // 1 << 6
        [Description("Event Management")]
        EventManagement = 0x0000000020,
        // 1 << 7
        [Description("Logistics Team")]
        Logistics = 0x0000000040,
        // 1 << 8
        [Description("TeamSpeak Administration")]
        TeamSpeakAdmin = 0x0000000080,
        // 1 << 9
        [Description("Holosite Support")]
        HolositeSupport = 0x0000000100,
        // 1 << 10
        [Description("Discord Management")]
        DiscordManagement = 0x0000000200,
        // 1 << 11
        [Description("Tech Support")]
        TechSupport = 0x0000000400,
        // 1 << 12
        [Description("Basic Combat Training Staff")]
        BCTStaff = 0x0000000800,
        // 1 << 13
        [Description("Private Training Instructor")]
        PrivateTrainingInstructor = 0x0000001000,
        // 1 << 14
        [Description("Upsilon Training Corps")]
        UTCStaff = 0x0000002000,
        // 1 << 15
        [Description("Qualification/MOS Training Staff")]
        QualTrainingStaff = 0x0000004000,
        // 1 << 16
        [Description("Server Management")]
        ServerManagement = 0x0000008000,
        // 1 << 17
        [Description("Auxiliary Mod Team")]
        AuxModTeam = 0x0000010000,
        // 1 << 18
        [Description("Public Affairs")]
        PublicAffairs = 0x0000020000,
        // 1 << 19
        [Description("Media/Streaming Outreach")]
        MediaOutreach = 0x0000040000,
        // 1 << 20
        [Description("News Team")]
        NewsTeam = 0x0000080000,
    }
}
