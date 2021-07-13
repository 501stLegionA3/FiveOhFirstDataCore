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

        public bool UseRankSearch { get; set; } = false;
        public TrooperRank? RankFilter { get; set; } = null;
        public string RankSearch { get; set; } = "";

        public bool UseUnitSearch { get; set; } = false;
        public Slot? UnitFilter { get; set; } = null;
        public string UnitSearch { get; set; } = "";

        public bool UseRoleSearch { get; set; } = false;
        public Role? RoleFilter { get; set; } = null;
        public string RoleSearch { get; set; } = "";
    }
}
