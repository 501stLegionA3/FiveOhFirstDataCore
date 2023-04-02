using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenFlightData : IAssignable<Trooper>
    {
        public Trooper FlightLead { get; set; }
        public WardenSectionData[] Sections { get; init; }
        public bool IsWardenToPilot { get; init; }

        public WardenFlightData(int sections, bool isWardenToPilot = false)
        {
            Sections = new WardenSectionData[sections];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new();
            IsWardenToPilot = isWardenToPilot;
        }

        public void Assign(Trooper item)
        {
            if (item.Flight == Flight.Alpha)
            {
                FlightLead = item;
            }
            else
            {
                int sections = (int?)item.Team ?? 0;
                if (sections < Sections.Length)
                    Sections[sections].Assign(item);
            }
        }
    }
}
