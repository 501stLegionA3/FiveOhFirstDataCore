using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Roster
{
    public class ChiefWarrantOfficerData : IAssignable<Trooper>
    {
        public Trooper CShopXO { get; set; }
        public Trooper SeniorRecruiter { get; set; }
        public Trooper InactiveReserves { get; set; }
        public Trooper NewsTeamLead { get; set; }
        public Trooper SeniorRosterClerk { get; set; }
        public Trooper ZeusOperator { get; set; }
        public Trooper AssaultCadre { get; set; }

        public void Assign(Trooper item)
        {
            throw new System.NotImplementedException();
        }
    }
}
