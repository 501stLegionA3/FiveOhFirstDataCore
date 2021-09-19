using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Account
{
    public class RecruitStatus
    {
        public Guid RecruitStatusId { get; set; }
        public bool OverSixteen { get; set; } = true;
        public int Age { get; set; }
        public bool ModsInstalled { get; set; } = true;
        public bool PossibleTroll { get; set; } = false;
        public PreferredRole PreferredRole { get; set; } = PreferredRole.Trooper;

        public int TrooperId { get; set; }
        public Trooper Trooper { get; set; }
    }
}
