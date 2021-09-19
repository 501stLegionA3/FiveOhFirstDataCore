using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class ZetaCompanyData : IAssignable<Trooper>
    {
        public Trooper Commander { get; set; }
        public Trooper XO { get; set; }
        public Trooper NCOIC { get; set; }
        public Trooper Adjutant { get; set; }
        public ZetaSectionData[] Sections { get; set; } = Array.Empty<ZetaSectionData>();
        public ZetaUTCSectionData UTCSection { get; set; }

        public ZetaCompanyData(int sections, int squads, int utcSquads)
        {
            Sections = new ZetaSectionData[sections];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new(squads);
            UTCSection = new(utcSquads);
        }

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
                if (val <= Sections.Length)
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
