using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenSection : IAssignable<Trooper>
    {
        public Trooper SectionLead { get; set; }
        public Trooper WardenTwo { get; set; }
        public Trooper WardenThree { get; set; }

        public void Assign(Trooper item)
        {
            if (item.Flight == Flight.Alpha)
                SectionLead = item;
            else if (item.Flight == Flight.Bravo)
                WardenTwo = item;
            else if (item.Flight == Flight.Charlie)
                WardenThree = item;
        }
    }
}
