using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class NickNameUpdate : UpdateBase
    {
        public Trooper ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }
        public string OldNickname { get; set; } = "";
        public string NewNickname { get; set; } = "";
    }
}
