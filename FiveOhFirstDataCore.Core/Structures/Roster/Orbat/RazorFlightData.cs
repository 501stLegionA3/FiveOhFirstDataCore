using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class RazorFlightData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public RazorSectionData[] Sections { get; set; } = new RazorSectionData[] { new(), new() };

        public void Assign(Trooper t)
        {
            if (t.Role == Role.Commander)
            {
                Commander = t;
                return;
            }
            // Convert the slot to a double value based on flight number
            var val = (double)decimal.Round((decimal)t.Slot / 10 % 10 % 1, 1);
            switch (val)
            {
                case 0.1:
                    Sections[0].Assign(t);
                    break;
                case 0.2:
                    Sections[1].Assign(t);
                    break;
            }
        }
    }

    public class RazorSectionData : IAssignable<Trooper>
    {
        public Trooper Alpha { get; set; }
        public Trooper Bravo { get; set; }
        public Trooper Charlie { get; set; }
        public Trooper Delta { get; set; }

        public void Assign(Trooper t)
        {
            switch (t.Flight)
            {
                case Flight.Alpha:
                    Alpha = t;
                    break;
                case Flight.Bravo:
                    Bravo = t;
                    break;
                case Flight.Charlie:
                    Charlie = t;
                    break;
                case Flight.Delta:
                    Delta = t;
                    break;
            }
        }
    }
}
