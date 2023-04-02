using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenData : IAssignable<Trooper>
    {
        public Trooper Master { get; set; }
        public Trooper Chief { get; set; }
        public WardenFlightData[] Flights { get; set; } = new WardenFlightData[] { new(2), new(2), new(1, true) };

        public void Assign(Trooper t)
        {
            switch (t.Role)
            {
                case Role.MasterWarden:
                    Master = t;
                    break;
                case Role.ChiefWarden:
                    Chief = t;
                    break;
                case Role.Warden:
                    try
                    {
                        int i = ((int)t.Slot % 10) - 1;
                        if (i < Flights.Length && i >= 0)
                            Flights[i].Assign(t);
                    }
                    catch
                    { 
                        // Catch execptions from old warden data.
                    }
                    break;
            }
        }
    }
}
