using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account
{
    public class RecruitStatus
    {
        public int RecruitStatusKey { get; set; }
        public bool OverSixteen { get; set; } = true;
        public bool ModsInstalled { get; set; } = true;

        public int TrooperId { get; set; }
        public virtual Trooper Trooper { get; set; }
    }
}
