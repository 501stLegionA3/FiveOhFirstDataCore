using ProjectDataCore.Data.Account;

namespace ProjectDataCore.Data.Structures.Model.Roster;

public class RosterSlotEditModel
{
    public Optional<int?> UserId { get; set; }
    public Guid? ParentTreeId { get; set; }
    public string? PositionName { get; set; }
}
