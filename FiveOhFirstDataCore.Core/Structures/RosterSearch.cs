using FiveOhFirstDataCore.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class RosterSearch
    {
        public string DesignationFilter { get; set; } = "";
        public bool Ascending { get; set; } = true;
        public int SortByColumn { get; set; } = 1;
        public string IdFilter { get; set; } = "";
        public string NickNameFilter { get; set; } = "";
        public TrooperRank? RankFilter { get; set; } = null;
        public Slot? UnitFilter { get; set; } = null;
        public Role? RoleFilter { get; set; } = null;
    }
}
