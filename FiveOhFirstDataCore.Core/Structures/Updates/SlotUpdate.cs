using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class SlotUpdate : UpdateBase
    {
        public Slot NewSlot { get; set; }
        public Team? NewTeam { get; set; }
        public Role? NewRole { get; set; }
        public Flight? NewFlight { get; set; }

        public Slot OldSlot { get; set; }
        public Team? OldTeam { get; set; }
        public Role? OldRole { get; set; }
        public Flight? OldFlight { get; set; }

        public List<Trooper> ApprovedBy { get; set; } = new();
    }
}
