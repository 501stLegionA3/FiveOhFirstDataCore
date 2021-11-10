using ProjectDataCore.Data.Account;

namespace ProjectDataCore.Data.Structures.Model.Roster;

public class RosterSlotEditModel : RosterObjectEditModel
{
    public Optional<int?> UserId { get; set; }
}
