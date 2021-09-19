using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class HailstormData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper XO { get; set; }
        public Trooper CShopCommander { get; set; }
        public Trooper CShopXO { get; set; }
        public Trooper NCOIC { get; set; }
        public Trooper Medic { get; set; }
        public Trooper RT { get; set; }

        public void Assign(Trooper t)
        {
            switch (t.Role)
            {
                case Role.Commander:
                    Commander = t;
                    break;
                case Role.XO:
                    XO = t;
                    break;
                case Role.CShopCommander:
                    CShopCommander = t;
                    break;
                case Role.CShopXO:
                    CShopXO = t;
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
            }
        }
    }
}
