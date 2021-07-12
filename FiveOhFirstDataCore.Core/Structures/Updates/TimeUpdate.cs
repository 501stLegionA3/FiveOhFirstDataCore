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
        public DateTime? NewGraduatedBCT { get; set; } = null;
        public DateTime? OldGraduatedBCT { get; set; } = null;

        public DateTime? NewGraduatedUTC { get; set; } = null;
        public DateTime? OldGraduatedUTC { get; set; } = null;

        public DateTime? NewBilletChange { get; set; } = null;
        public DateTime? OldBilletChange { get; set; } = null;

        public DateTime? NewPromotion { get; set; } = null;
        public DateTime? OldPromotion { get; set; } = null;

        public DateTime? NewStartOfService { get; set; } = null;
        public DateTime? OldStartOfService { get; set; } = null;

        public Trooper ChangedBy { get; set; }
        public int? ChangedById { get; set; }
    }
}
