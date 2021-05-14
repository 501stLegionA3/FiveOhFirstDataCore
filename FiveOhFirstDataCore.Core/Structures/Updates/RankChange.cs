using FiveOhFirstDataCore.Core.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class RankChange : UpdateBase
    {
        public int ChangedFrom { get; set; }
        public int ChangedTo { get; set; }

        public virtual Trooper ChangedBy { get; set; }
        public int ChangedById { get; set; }
    }
}
