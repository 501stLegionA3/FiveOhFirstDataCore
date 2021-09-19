using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class NickNameUpdate : UpdateBase
    {
        public Trooper ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }
        public string OldNickname { get; set; } = "";
        public string NewNickname { get; set; } = "";
    }
}
