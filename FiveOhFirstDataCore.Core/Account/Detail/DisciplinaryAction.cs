namespace FiveOhFirstDataCore.Data.Account.Detail
{
    public class DisciplinaryAction
    {
        public Guid DAID { get; set; }
        public Trooper FiledBy { get; set; }
        public int FiledById { get; set; }
        public Trooper FiledTo { get; set; }
        public int FiledToId { get; set; }
        public Trooper FiledAgainst { get; set; }
        public int FiledAgainstId { get; set; }
        public DateTime FiledOn { get; set; }
        public DateTime OccurredOn { get; set; }
        public string Location { get; set; }
        public string Situation { get; set; }
        public List<Trooper> Witnesses { get; set; }
        public string IncidentReport { get; set; }
        public string Summary { get; set; }
        public string Notes { get; set; }
        public string Recommendation { get; set; }
        public bool ActionTaken { get; set; } = false;
        public bool WillTakeAction { get; set; } = true;
    }
}
