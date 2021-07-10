using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components.Base
{
    public class NoticePageModel : ComponentBase
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
        public int ItemsPerPage { get; private set; } = 5;

        public int Items { get; set; }
        public int Segments { get
            {
                return (int)Math.Ceiling(Items / (double)ItemsPerPage);
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

        public void NextPage()
        {
            if (PageIndex + 1 <= Segments)
            {
                PageIndex++;
                InvokeAsync(StateHasChanged);
            }
        }

        public void PreviousPage()
        {
            if (PageIndex - 1 > 0)
            {
                PageIndex--;
                InvokeAsync(StateHasChanged);
            }
        }

        public void SetPage(int index)
        {
            PageIndex = index;
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
            return PageIndex < Segments;
        }
    }
}
