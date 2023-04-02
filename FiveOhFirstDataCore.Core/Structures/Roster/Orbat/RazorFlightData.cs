using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class RazorFlightData : IAssignable<Trooper>
    {
        public Trooper FlightLeader { get; set; }
        public Trooper SectionLeader { get; set; }
        public Trooper Charlie { get; set; }
        public Trooper Delta { get; set; }
        public List<Trooper> Echo { get; set; } = new();

        public void Assign(Trooper t)
        {
            switch (t.Flight)
            {
                case Flight.Alpha:
                    FlightLeader = t;
                    break;
                case Flight.Bravo:
                    SectionLeader = t;
                    break;
                case Flight.Charlie:
                    Charlie = t;
                    break;
                case Flight.Delta:
                    Delta = t;
                    break;
                case Flight.Echo:
                    Echo.Add(t);
                    break;
            }
        }
    }
}
