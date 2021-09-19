using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class CompanyData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper XO { get; set; }
        public Trooper NCOIC { get; set; }
        public Trooper Medic { get; set; }
        public Trooper RT { get; set; }
        public Trooper ARC { get; set; }
        public Trooper Adjutant { get; set; }
        public PlatoonData[] Platoons { get; set; }

        public CompanyData(int platoons, int squads)
        {
            Platoons = new PlatoonData[platoons];
            for (int i = 0; i < Platoons.Length; i++)
                Platoons[i] = new(squads);
        }

        public void Assign(Trooper t)
        {
            var val = (int)t.Slot / 10 % 10;

            if (val == 0)
            {
                switch (t.Role)
                {
                    case Role.Commander:
                        Commander = t;
                        break;
                    case Role.XO:
                        XO = t;
                        break;
                    case Role.NCOIC:
                        NCOIC = t;
                        break;
                    case Role.Medic:
                        Medic = t;
                        break;
                    case Role.RTO:
                        RT = t;
                        break;
                    case Role.ARC:
                        ARC = t;
                        break;
                    case Role.Adjutant:
                        Adjutant = t;
                        break;
                }
            }
            if (val >= 1 && val <= Platoons.Length)
            {
                Platoons[val - 1].Assign(t);
            }
        }
    }
}
