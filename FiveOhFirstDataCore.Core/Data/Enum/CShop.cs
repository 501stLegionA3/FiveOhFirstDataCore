using System;
using System.Collections.Generic;
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
        RosterStaff = 0x0000000001,
        // 1 << 2
        DocMainCom = 0x0000000002,
        // 1 << 3
        RecruitingStaff = 0x0000000004,
        // 1 << 4
        ReturningMemberStaff = 0x0000000008,
        // 1 << 5
        CampaignManagment = 0x0000000010,
        // 1 << 6
        EventManagment = 0x0000000020,
        // 1 << 7
        Logistics = 0x0000000040,
        // 1 << 8
        TeamSpeakAdmin = 0x0000000080,
        // 1 << 9
        HolositeSupport = 0x0000000100,
        // 1 << 10
        DiscordManagment = 0x0000000200,
        // 1 << 11
        TechSupport = 0x0000000400,
        // 1 << 12
        BCTStaff = 0x0000000800,
        // 1 << 13
        PrivateTrainingInstructor = 0x0000001000,
        // 1 << 14
        UTCStaff = 0x0000002000,
        // 1 << 15
        QualTrainingStaff = 0x0000004000,
        // 1 << 16
        ServerManagment = 0x0000008000,
        // 1 << 17
        AuxModTeam = 0x0000010000,
        // 1 << 18
        PublicAffairs = 0x0000020000,
        // 1 << 19
        MediaOutreach = 0x0000040000,
        // 1 << 20
        NewsTeam = 0x0000080000,
    }
}
