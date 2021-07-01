using FiveOhFirstDataCore.Core.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class TimeUpdate : UpdateBase
    {
        public DateTime NewGraduatedBCT { get; set; }
        public DateTime OldGraduatedBCT { get; set; }

        public DateTime NewGraduatedUTC { get; set; }
        public DateTime OldGraduatedUTC { get; set; }

        public DateTime NewBilletChange { get; set; }
        public DateTime OldBilletChange { get; set; }

        public DateTime NewPromotion { get; set; }
        public DateTime OldPromotion { get; set; } 

        public DateTime NewStartOfService { get; set; }
        public DateTime OldStartOfService { get; set; }

        public Trooper ChangedBy { get; set; }
        public int ChangedById { get; set; }
    }
}
