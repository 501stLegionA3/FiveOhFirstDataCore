using FiveOhFirstDataCore.Core.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class RecruitmentChange : UpdateBase
    {
        public Trooper RecruitedBy { get; set; }
        public int RecruitedById { get; set; }
    }
}
