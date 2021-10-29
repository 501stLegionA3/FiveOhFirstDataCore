using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class WardenData : IAssignable<Trooper>
    {
        public Trooper Master { get; set; }
        public Trooper Chief { get; set; }
        public WardenSection[] Wardens { get; set; } = new WardenSection[] { new(), new() };

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
                        if (i < Wardens.Length && i >= 0)
                            Wardens[i].Assign(t);
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
