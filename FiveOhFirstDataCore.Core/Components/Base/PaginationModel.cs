
using Microsoft.AspNetCore.Components;

namespace FiveOhFirstDataCore.Data.Components.Base
{
    public class PaginationModel : ComponentBase
    {
        /// <summary>
        /// The current max item indxe for this page
        /// </summary>
        public int CurrentPageCap
        {
            get
            {
                return PageIndex * ItemsPerPage;
            }
        }
        /// <summary>
        /// The current min item index for this page
        /// </summary>
        public int CurrentPageStart
        {
            get
            {
                return CurrentPageCap - ItemsPerPage;
            }
        }
        /// <summary>
        /// The current page to display
        /// </summary>
        public int PageIndex { get; private set; } = 1;
        /// <summary>
        /// The ammount of items per page
        /// </summary>
        public int ItemsPerPage { get; set; } = 5;
        /// <summary>
        /// The items on this page
        /// </summary>
        public IReadOnlyList<dynamic> Items = new List<dynamic>();
        /// <summary>
        /// The method that is called to load the next batch of items
        /// </summary>
        private Func<int, int, object[], Task<IReadOnlyList<object>>>? LoadNextBatch { get; set; }
        /// <summary>
        /// Extra parameters for the Load Next Batch method.
        /// </summary>
        private object[] Params { get; set; } = Array.Empty<object>();
        /// <summary>
        /// The method that is called to get the total count of items.
        /// </summary>
        private Func<object[], Task<int>>? GetCount { get; set; }
        /// <summary>
        /// The item count for all items that can be displayed
        /// </summary>
        private int ItemCount { get; set; } = 0;

        /// <summary>
        /// The ammount of items on either side of the current page.
        /// </summary>
        public int PaginationCounterItemsHalf { get; set; } = 5;
        /// <summary>
        /// The ammount of pages that can be displayed.
        /// </summary>
        public int Segments
        {
            get
            {
                return (int)Math.Ceiling((double)ItemCount / ItemsPerPage);
            }
        }
        /// <summary>
        /// The start value for the pagination controller
        /// </summary>
        public int PaginationCounterStart
        {
            get
            {
                var i = PageIndex - PaginationCounterItemsHalf;
                if (i < 1) i = 1;
                return i;
            }
        }
        /// <summary>
        /// The end value for the pagination controller
        /// </summary>
        public int PaginationCounterEnd
        {
            get
            {
                var i = PageIndex + PaginationCounterItemsHalf;
                if (i > Segments) i = Segments;
                return i;
            }
        }
        /// <summary>
        /// Initalize the Paignation Model.
        /// </summary>
        /// <remarks>
        /// This method calls <see cref="LoadPageAsync"/> and sets the <see cref="LoadNextBatch"/> and <see cref="GetCount"/>
        /// values for futhre use of the model. It also sets the items per page and starting page index.
        /// <br /><br />
        /// This overload expects a function with the return value of <see cref="Task{TResult}"/> where TResult is 
        /// a <see cref="List{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items to batch load.</typeparam>
        /// <param name="loader">The async fucntion that returns a <see cref="IReadOnlyList{T}"/> for the current page.</param>
        /// <param name="counts">The function that returns an <see cref="int"/> of the total item count</param>
        /// <param name="parameters">Extra parameters for the load next batch function.</param>
        /// <param name="itemsPerPage">Sets how many items will be displayed per page. Defaults to 5.</param>
        /// <param name="startingPage">Sets the starting page index. Must be above 0. Defaults to 1.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        public async Task InitalizeAsync<T>(Func<int, int, object[], Task<IReadOnlyList<T>>> loader, Func<object[], Task<int>> counts,
            object[]? parameters = null, int itemsPerPage = 5, int startingPage = 1)
        {
            SetBatchLoader(async (x, y, z) => (await loader.Invoke(x, y, z)).Cast<object>().ToList());
            SetCountMethod(counts);

            ItemsPerPage = itemsPerPage;
            Params = parameters ?? Array.Empty<object>();

            if (startingPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(startingPage),
                    $"Expected starting page to be above 0, found {startingPage}");

            PageIndex = startingPage;

            await LoadPageAsync();
        }

        /// <summary>
        /// Initalize the Paignation Model.
        /// </summary>
        /// <remarks>
        /// This method calls <see cref="LoadPageAsync"/> and sets the <see cref="LoadNextBatch"/> and <see cref="GetCount"/>
        /// values for futhre use of the model. It also sets the items per page and starting page index.
        /// <br /><br />
        /// This overload expects a function with the return value of <see cref="List{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items to batch load.</typeparam>
        /// <param name="loader">The fucntion that returns a <see cref="IReadOnlyList{T}"/> for the current page.</param>
        /// <param name="counts">The function that returns an <see cref="int"/> of the total item count</param>
        /// <param name="parameters">Extra parameters for the load next batch function.</param>
        /// <param name="itemsPerPage">Sets how many items will be displayed per page. Defaults to 5.</param>
        /// <param name="startingPage">Sets the starting page index. Must be above 0. Defaults to 1.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        public async Task InitalizeAsync<T>(Func<int, int, object[], IReadOnlyList<T>> loader, Func<object[], Task<int>> counts,
            object[]? parameters = null, int itemsPerPage = 5, int startingPage = 1)
            => await InitalizeAsync((x, y, z) => Task.FromResult(loader.Invoke(x, y, z)), counts, parameters, itemsPerPage, startingPage);
        /// <summary>
        /// Resets the page index to 1 and reloads the page.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action</returns>
        public async Task ResetPaignationAsync()
        {
            PageIndex = 1;
            await LoadPageAsync();
        }
        /// <summary>
        /// Changes the ammoutn of items per page.
        /// </summary>
        /// <param name="newCount">The new ammount of items per page.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task represetning this action.</returns>
        public async Task ChangeItemsPerPageAsync(int newCount)
        {
            ItemsPerPage = newCount;
            await LoadPageAsync();
        }
        /// <summary>
        /// Moves to the next page if there is a next page.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        public virtual async Task NextPage()
        {
            if (PageIndex + 1 <= Segments)
            {
                PageIndex++;

                await LoadPageAsync();
            }
        }
        /// <summary>
        /// Moves to the previous page if there is a previous page.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        public virtual async Task PreviousPage()
        {
            if (PageIndex - 1 > 0)
            {
                PageIndex--;

                await LoadPageAsync();
            }
        }
        /// <summary>
        /// Sets the page to the given index.
        /// </summary>
        /// <param name="index">The page index to move to. Must be above 0 and below or equal to the segments count.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this aciton.</returns>
        public virtual async Task SetPage(int index)
        {
            if (index > Segments)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"{nameof(index)} was higher than the segemnts count [{Segments}]");

            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"{nameof(index)} was lower than 0.");

            PageIndex = index;

            await LoadPageAsync();
        }
        /// <summary>
        /// Loads the current page.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        private async Task LoadPageAsync()
        {
            if (LoadNextBatch is not null)
            {
                Items = await LoadNextBatch.Invoke(CurrentPageStart, CurrentPageCap, Params);
                await UpdateItemCountAsync();
                InvokeStateHasChanged();
            }
            else throw new ArgumentNullException(nameof(LoadNextBatch), "The load next batch value has not been set.");
        }
        /// <summary>
        /// Updates the Item Count variable with the current item counts.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>A task representing this action.</returns>
        private async Task UpdateItemCountAsync()
        {
            if (GetCount is not null)
                ItemCount = await GetCount.Invoke(Params);
            else throw new ArgumentNullException(nameof(GetCount), "Get count method can not be null");
        }
        /// <summary>
        /// Invokes <see cref="StateHasChanged"/> on the render thread.
        /// </summary>
        public void InvokeStateHasChanged()
        {
            InvokeAsync(StateHasChanged);
        }
        /// <summary>
        /// Sets the batch loader.
        /// </summary>
        /// <param name="loader">The async function that loads the current page.</param>
        public void SetBatchLoader(Func<int, int, object[], Task<IReadOnlyList<object>>> loader)
            => LoadNextBatch = loader;
        /// <summary>
        /// Sets the batch loader.
        /// </summary>
        /// <param name="loader">The function that loads the current page.</param>
        public void SetBatchLoader(Func<int, int, object[], IReadOnlyList<object>> loader)
            => LoadNextBatch = (x, y, z) => Task.FromResult(loader.Invoke(x, y, z));
        /// <summary>
        /// Sets the count method.
        /// </summary>
        /// <param name="counts">The async function that gets the current total item count.</param>
        public void SetCountMethod(Func<object[], Task<int>> counts)
            => GetCount = counts;
    }
}
