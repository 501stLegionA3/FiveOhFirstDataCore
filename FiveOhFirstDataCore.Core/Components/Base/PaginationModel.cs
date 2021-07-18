using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components.Base
{
    public class PaginationModel : ComponentBase
    {
        public int CurrentPageCap
        {
            get
            {
                return PageIndex * ItemsPerPage;
            }
        }
        public int CurrentPageStart
        {
            get
            {
                return CurrentPageCap - ItemsPerPage;
            }
        }

        public int PageIndex { get; private set; } = 1;
        public int ItemsPerPage { get; set; } = 5;

        public int ItemCount { get; set; }
        public bool KnowItemCount { get; set; } = false;

        public int Segments { get
            {
                return (int)Math.Ceiling(ItemCount / (double)ItemsPerPage);
            }
        }

        public void ResetPaignation()
        {
            PageIndex = 1;
            InvokeAsync(StateHasChanged);
        }

        public void ChangeItemsPerPage(int newCount)
        {
            ItemsPerPage = newCount;
            InvokeAsync(StateHasChanged);
        }

        public virtual Task NextPage()
        {
            if (!KnowItemCount || PageIndex + 1 <= Segments)
            {
                PageIndex++;
            }

            return Task.CompletedTask;
        }

        public virtual Task PreviousPage()
        {
            if (PageIndex - 1 > 0)
            {
                PageIndex--;
            }

            return Task.CompletedTask;
        }

        public virtual Task SetPage(int index)
        {
            PageIndex = index;

            return Task.CompletedTask;
        }

        public (bool, bool) GetNextPrevSegmentChecks()
        {
            return (HasPerviousSegment(), HasNextSegment());
        }

        public bool HasPerviousSegment()
        {
            return PageIndex > 1;
        }

        public bool HasNextSegment()
        {
            return !KnowItemCount || PageIndex < Segments;
        }

        public void InvokeStateHasChanged()
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
