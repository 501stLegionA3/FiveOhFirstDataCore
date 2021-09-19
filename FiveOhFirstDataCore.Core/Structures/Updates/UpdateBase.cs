using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public abstract class UpdateBase
    {
        public Guid ChangeId { get; set; }
        public DateTime ChangedOn { get; set; }
        public Trooper ChangedFor { get; set; }
        public int ChangedForId { get; set; }
        public bool SubmittedByRosterClerk { get; set; } = false;
        public bool RevertChange { get; set; } = false;
    }
}
