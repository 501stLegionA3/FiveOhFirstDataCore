using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class ZetaOrbatData : IAssignable<Trooper>
    {
        public HailstormData Hailstorm { get; set; } = new();

        public ZetaCompanyData Zeta { get; set; } = new(1, 4, 4);

        public void Assign(Trooper item)
        {
            if (item.Slot == Slot.Hailstorm)
                Hailstorm.Assign(item);
            else if (item.Slot >= Slot.ZetaCompany && item.Slot < Slot.InactiveReserve)
                Zeta.Assign(item);
        }
    }
}
