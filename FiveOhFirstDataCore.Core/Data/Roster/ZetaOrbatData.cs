using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class ZetaOrbatData : IAssignable<Trooper>
    {
        public HailstormData Hailstorm { get; set; } = new();

        public ZetaCompanyData Zeta { get; set; } = new();

        public void Assign(Trooper item)
        {
            if (item.Slot == Slot.Hailstorm)
                Hailstorm.Assign(item);
            else if (item.Slot <= Slot.ZetaCompany && item.Slot > Slot.InactiveReserve)
                Zeta.Assign(item);
        }
    }
}
