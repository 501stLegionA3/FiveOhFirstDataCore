using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class RecruitmentUpdate : UpdateBase
    {
        public Trooper RecruitedBy { get; set; }
        public int? RecruitedById { get; set; }
    }
}
