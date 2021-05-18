using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account.Detail
{
    public class DisciplinaryAction
    {
        public Guid DAID { get; set; }
        public virtual Trooper FiledBy { get; set; }
        public virtual int FiledById { get; set; }
        public virtual Trooper FiledTo { get; set; }
        public virtual int FiledToId { get; set; }
        public virtual Trooper FiledAgainst { get; set; }
        public virtual int FiledAgainstId { get; set; }
        public DateTime FiledOn { get; set; }
        public DateTime OccouredOn { get; set; }
        public string Location { get; set; }
        public string Situation { get; set; }
        public virtual List<Trooper> Witnesses { get; set; }
        public string IncidentReport { get; set; }
        public string Summary { get; set; }
        public string Notes { get; set; }
        public string Recommendation { get; set; }
        public bool ActionTaken { get; set; } = false;
        public bool WillTakeAction { get; set; } = true;
    }
}
