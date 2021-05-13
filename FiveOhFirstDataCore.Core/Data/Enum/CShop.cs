using System;
using System.Collections.Concurrent;
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
    }

    public static class CShopExtensions
    {
        public static Dictionary<CShop, Dictionary<string, HashSet<string>>> ClaimsTree { get; private set; } = new()
        {
            // C1
            { CShop.RosterStaff, new() { { CShop.RosterStaff.AsFull(), new() { "Lead", "Assistant", "Sr", "Clerk", "Jr" } } } },
            { CShop.DocMainCom, new() { { CShop.DocMainCom.AsFull(), new() { "Lead", "Assistant", "Curator", "Proofreader", "Distribution Overseer", "Assistant" } } } },
            { CShop.RecruitingStaff, new() { { CShop.RecruitingStaff.AsFull(), new() { "Lead", "Assistant", "Sr", "Recruiter" } } } },
            { CShop.ReturningMemberStaff, new() { { CShop.ReturningMemberStaff.AsFull(), new() { "Lead", "Assistant", "Staff" } } } },
            { CShop.MedalsStaff, new() { { CShop.MedalsStaff.AsFull(), new() { "Lead", "Assistant", "Staff" } } } },

            // C3
            { CShop.CampaignManagement, new() { 
                { CShop.CampaignManagement.AsFull(), new() { "DeptAssistant" } },
                { "Story Writer", new() { "Lead", "Assistant", "Writer" } },
                { "Zeus", new() { "Lead", "Assistant", "Zeus" } },
                { "Mission Builder", new() { "Lead", "Assistant", "Sr", "Builder", "Jr"} },
                { "Artisan", new() { "Lead", "Assistant", "Artisan" } },
                { "Logistics", new() { "Staff" } }
            } },
            { CShop.EventManagement, new() { { CShop.EventManagement.AsFull(), new() { "Lead", "Assistant", "Documentation", "Host", "Helper" } } } },

            // C4
            { CShop.Logistics, new() { { CShop.Logistics.AsFull(), new() { "Donations", "Patreon", "TeamSpeak", "Website" } } } },

            // C5
            { CShop.TeamSpeakAdmin, new() { { CShop.TeamSpeakAdmin.AsFull(), new() { "Lead", "Assistant", "Staff" } } } },
            { CShop.HolositeSupport, new() { { CShop.HolositeSupport.AsFull(), new() { "Lead", "Staff" } } } },
            { CShop.DiscordManagement, new() { { CShop.DiscordManagement.AsFull(), new() { "Lead", "Staff" } } } },
            { CShop.TechSupport, new() { { CShop.TechSupport.AsFull(), new() { "Lead", "Assistant", "Staff" } } } },

            // C6
            { CShop.BCTStaff, new() { { CShop.BCTStaff.AsFull(), new() { "Lead", "Assistant", "Sr", "Instructor", "Cadre" } } } },
            { CShop.PrivateTrainingInstructor, new() { { CShop.PrivateTrainingInstructor.AsFull(), new() { "Lead", "Assistant", "Instructor", "Cadre" } } } },
            { CShop.UTCStaff, new() { { CShop.UTCStaff.AsFull(), new() { "Lead", "Assistant", "Documentation", "Phase Blue Lead", "Chief", "Sr", "Instructor", "Jr" } } } },
            { CShop.QualTrainingStaff, new() { 
                { CShop.QualTrainingStaff.AsFull(), new() { "Lead" } },
                { "RTO", new() { "Lead", "Instructor", "Cadre" } },
                { "Medical", new() { "Lead", "Instructor", "Cadre" } },
                { "Assault", new() { "Lead", "Instructor", "Cadre" } },
                { "Support", new() { "Lead", "Instructor", "Cadre" } },
                { "Marksman", new() { "Lead", "Instructor", "Cadre" } },
                { "Grenadier", new() { "Lead", "Instructor", "Cadre" } },
                { "Jump-Master", new() { "Lead", "Instructor", "Cadre" } },
                { "Combat Engineer", new() { "Lead", "Instructor", "Cadre" } }
            } },

            // C7
            { CShop.ServerManagement, new() { { CShop.ServerManagement.AsFull(), new() { "Lead", "Assistant", "Staff", "Minecraft", "Space Engineers" } } } },
            { CShop.AuxModTeam, new() { { CShop.AuxModTeam.AsFull(), new() { "Lead Developer", "Developer", "Texture Lead", "Texture Team" } } } },

            // C8
            { CShop.PublicAffairs, new() { { CShop.PublicAffairs.AsFull(), new() { "Units", "TCW", "Steam" } } } },
            { CShop.MediaOutreach, new() { 
                { CShop.MediaOutreach.AsFull(), new() { "Lead", "Assistatn", "Mod" } },
                { "YouTube", new() { "Lead", "Assistant", "Staff" } },
                { "Facebook", new() { "Staff" } },
                { "Twitter", new() { "Staff" } },
                { "TikTok", new() { "Lead", "Staff" } }
            } },
            { CShop.NewsTeam, new() { { CShop.NewsTeam.AsFull(), new() { "Lead", "Managing Editor", "Copy Editor", "News Editor", "Graphics Editor", "Writer", "Photographer"} } } }
        };
    }
}
