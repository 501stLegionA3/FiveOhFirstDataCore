using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class RecruitmentUpdate : UpdateBase
    {
        public Trooper RecruitedBy { get; set; }
        public int? RecruitedById { get; set; }
    }
}
