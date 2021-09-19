using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Structures
{
    public class RecruitSearch
    {
        public bool Ascending { get; set; } = default;
        public int SortByColumn { get; set; } = 0;
        public string NickNameFilter { get; set; } = string.Empty;
        public string IdFilter { get; set; } = string.Empty;

        public PreferredRole? PreferredRole { get; set; } = null;
    }
}
