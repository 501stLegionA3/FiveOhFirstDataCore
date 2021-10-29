using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenSection : IAssignable<Trooper>
    {
        public Trooper SectionLead { get; set; }
        public Trooper[] TeamOne { get; set; } = new Trooper[4];
        public Trooper[] TeamTwo { get; set; } = new Trooper[4];

        public void Assign(Trooper item)
        {
            if (item.Slot == Slot.WardenTwo)
            {
                // Warden To Pilot Program.
                if (item.Flight is not null)
                    TeamOne[(int)item.Flight] = item;
            }
            else if (item.Team is null)
            {
                SectionLead = item;
            }
            else if (item.Team == Team.Alpha)
            {
                if (item.Flight is not null)
                    TeamOne[(int)item.Flight] = item;
            }
            else if (item.Team == Team.Bravo)
            {
                if (item.Flight is not null)
                    TeamTwo[(int)item.Flight] = item;
            }
        }
    }
}
