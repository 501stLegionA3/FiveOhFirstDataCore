
using FiveOhFirstDataCore.Core.Structures.Updates;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<dynamic> Items = new();

        private Func<int, int, Task<List<object>>>? LoadNextBatch { get; set; }
        private Func<Task<int>>? GetCount { get; set; }

        private int ItemCount { get; set; } = 0;

        /// <summary>
        /// The ammount of items on either side of the current page.
        /// </summary>
        public int PaginationCounterItemsHalf { get; set; } = 5;

        public int Segments
        {
            get
            {
                return (int)Math.Ceiling((double)ItemCount / ItemsPerPage);
            }
        }

        public int PaginationCounterStart
        {
            get
            {
                var i = PageIndex - PaginationCounterItemsHalf;
                if (i < 1) i = 1;
                return i;
            }
        }

        public int PaginationCounterEnd
        {
            get
            {
                var i = PageIndex + PaginationCounterItemsHalf;
                if (i > Segments) i = Segments;
                return i;
            }
        }

        public async Task InitalizeAsync<T>(Func<int, int, Task<List<T>>> loader, Func<Task<int>> counts,
            int itemsPerPage = 5, int startingPage = 1)
        {
            SetBatchLoader(async (x, y) => (await loader.Invoke(x, y)).Cast<object>().ToList());
            SetCountMethod(counts);

            ItemsPerPage = itemsPerPage;
            PageIndex = startingPage;

            await LoadPageAsync();
        }

        public async Task InitalizeAsync<T>(Func<int, int, List<T>> loader, Func<Task<int>> counts,
            int itemsPerPage = 5, int startingPage = 1)
            => await InitalizeAsync((x, y) => Task.FromResult(loader.Invoke(x, y)), counts, itemsPerPage, startingPage);

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

        public virtual async Task NextPage()
        {
            if (PageIndex + 1 <= Segments)
            {
                PageIndex++;

                await LoadPageAsync();
            }
        }

        public virtual async Task PreviousPage()
        {
            if (PageIndex - 1 > 0)
            {
                PageIndex--;

                await LoadPageAsync();
            }
        }

        public virtual async Task SetPage(int index)
        {
            PageIndex = index;

            await LoadPageAsync();
        }

        private async Task LoadPageAsync()
        {
            if (LoadNextBatch is not null)
            {
                Items = await LoadNextBatch.Invoke(CurrentPageStart, CurrentPageCap);
                await UpdateItemCountAsync();
                InvokeStateHasChanged();
            }
            else throw new ArgumentNullException(nameof(LoadNextBatch), "The load next batch value has not been set.");
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

        private async Task UpdateItemCountAsync()
        {
            if (GetCount is not null)
                ItemCount = await GetCount.Invoke();
            else throw new ArgumentNullException(nameof(GetCount), "Get count method can not be null");
        }

        public void InvokeStateHasChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void SetBatchLoader(Func<int, int, Task<List<object>>> loader)
            => LoadNextBatch = loader;

        public void SetBatchLoader(Func<int, int, List<object>> loader)
            => LoadNextBatch = (x, y) => Task.FromResult(loader.Invoke(x, y));

        public void SetCountMethod(Func<Task<int>> counts)
            => GetCount = counts;
    }
}
