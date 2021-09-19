using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class OrbatData : IAssignable<Trooper>
    {
        public HailstormData Hailstorm { get; set; } = new();
        public CompanyData Avalanche { get; set; } = new(3, 3);
        public CompanyData Cyclone { get; set; } = new(3, 3);
        public CompanyData Acklay { get; set; } = new(1, 3);
        public MynockDetachmentData Mynock { get; set; } = new();
        public RazorSquadronData Razor { get; set; } = new();
        public MilitaryPolice MilitaryPolice { get; set; } = new();
        public ChiefWarrantOfficerData ChiefWarrantOfficers { get; set; } = new();

        public void Assign(Trooper t)
        {
            if (t.Slot == Slot.Hailstorm)
                Hailstorm.Assign(t);
            else if (t.Slot < Slot.CycloneCompany && t.Slot >= Slot.AvalancheCompany)
                Avalanche.Assign(t);
            else if (t.Slot < Slot.AcklayCompany && t.Slot >= Slot.CycloneCompany)
                Cyclone.Assign(t);
            else if (t.Slot < Slot.Mynock && t.Slot >= Slot.AcklayCompany)
                Acklay.Assign(t);
            else if (t.Slot < Slot.Razor && t.Slot >= Slot.Mynock)
                Mynock.Assign(t);
            else if (t.Slot < Slot.ZetaCompany && t.Slot >= Slot.Razor)
                Razor.Assign(t);
        }
    }
}
