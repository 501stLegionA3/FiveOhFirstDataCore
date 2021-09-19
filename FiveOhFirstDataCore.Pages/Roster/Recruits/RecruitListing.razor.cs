using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Roster.Recruits
{
    public partial class RecruitListing
    {
        [Parameter]
        public List<Trooper> Troopers { get; set; } = new();

        private List<Trooper> Filtered { get; set; } = new();

        private string D1 { get; set; } = "";
        private string D2 { get; set; } = "";

        private RecruitSearch Search { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            await BuildFilteredList();
        }

        private string GetSortButtonClass(int col)
        {
            if (col == Search.SortByColumn)
                return Search.Ascending ? "oi-chevron-top" : "oi-chevron-bottom";
            else return "oi-menu";
        }

        private async Task OnSearchDirectionChanged(int col)
        {
            if (col == Search.SortByColumn)
                Search.Ascending = !Search.Ascending;
            else
            {
                Search.SortByColumn = col;
                Search.Ascending = default;
            }

            await BuildFilteredList();
        }

        private async Task OnNickChange(ChangeEventArgs e)
        {
            Search.NickNameFilter = (string?)e.Value ?? "";

            await BuildFilteredList();
        }

        private async Task OnIdChange(ChangeEventArgs e)
        {
            Search.IdFilter = (string?)e.Value ?? "";

            await BuildFilteredList();
        }

        private async Task OnPrefferedChange(ChangeEventArgs e)
        {
            var val = ((string?)e.Value) ?? "";

            if (string.IsNullOrWhiteSpace(val)) Search.PreferredRole = null;
            else Search.PreferredRole = val.ValueFromString<PreferredRole>();

            await BuildFilteredList();
        }

        private Task BuildFilteredList()
        {
            Filtered = Troopers;

            if (!string.IsNullOrWhiteSpace(Search.NickNameFilter))
            {
                Filtered = Filtered.Where(x => x.NickName.StartsWith(Search.NickNameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(Search.IdFilter))
            {
                Filtered = Filtered.Where(x => x.BirthNumber.ToString().StartsWith(Search.IdFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (Search.PreferredRole is not null)
            {
                Filtered = Filtered.Where(x => x.RecruitStatus?.PreferredRole == Search.PreferredRole).ToList();
            }

            if (Search.Ascending)
            {
                switch (Search.SortByColumn)
                {
                    case 1:
                        Filtered.Sort((x, y) => x.NickName.CompareTo(y.NickName));
                        break;
                    case 2:

                        Filtered.Sort((x, y) =>
                        {
                            if (x.RecruitStatus is null && y.RecruitStatus is null)
                                return 0;

                            if (x.RecruitStatus is null)
                                return 1;
                            if (y.RecruitStatus is null)
                                return -1;

                            return x.RecruitStatus.PreferredRole.CompareTo(y.RecruitStatus.PreferredRole);
                        });
                        break;
                    case 0:
                    default:
                        Filtered.Sort((x, y) => x.Id.CompareTo(y.Id));
                        break;
                }
            }
            else
            {
                switch (Search.SortByColumn)
                {
                    case 1:
                        Filtered.Sort((x, y) => y.NickName.CompareTo(x.NickName));
                        break;
                    case 2:
                        Filtered.Sort((x, y) =>
                        {
                            if (x.RecruitStatus is null && y.RecruitStatus is null)
                                return 0;

                            if (x.RecruitStatus is null)
                                return -1;
                            if (y.RecruitStatus is null)
                                return 1;

                            return y.RecruitStatus.PreferredRole.CompareTo(x.RecruitStatus.PreferredRole);
                        });
                        break;
                    case 0:
                    default:
                        Filtered.Sort((x, y) => y.Id.CompareTo(x.Id));
                        break;
                }
            }

            StateHasChanged();

            return Task.CompletedTask;
        }
    }
}
