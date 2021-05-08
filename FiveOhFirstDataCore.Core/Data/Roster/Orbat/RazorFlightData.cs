using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class RazorFlightData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public RazorSectionData[] Sections { get; set; } = new RazorSectionData[] { new(), new() };

        public void Assign(Trooper t)
        {
            switch (t.Team)
            {
                case null:
                    if (t.Role == Role.Commander)
                        Commander = t;
                    break;
                case Team.Alpha:
                    Sections[0].Assign(t);
                    break;
                case Team.Bravo:
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
