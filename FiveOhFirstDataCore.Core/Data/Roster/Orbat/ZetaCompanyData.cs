using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public class ZetaCompanyData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper XO { get; set; }
        public Trooper NCOIC { get; set; }
        public Trooper Adjutant { get; set; }
        public ZetaSectionData[] Sections { get; set; } = new ZetaSectionData[] { new(), new() };
        public ZetaUTCSectionData UTCSection { get; set; } = new();

        public void Assign(Trooper item)
        {
            int val = (int)item.Slot / 10 % 10;
            if (val == 0)
            {
                switch (item.Role)
                {
                    case Role.Commander:
                        Commander = item;
                        break;
                    case Role.XO:
                        XO = item;
                        break;
                    case Role.NCOIC:
                        NCOIC = item;
                        break;
                    case Role.Adjutant:
                        Adjutant = item;
                        break;
                }
            }
            else
            {
                if (val < 3)
                {
                    Sections[val - 1].Assign(item);
                }
                else
                {
                    UTCSection.Assign(item);
                }
            }
        }
    }
}
