using ProjectDataCore.Data.Structures.Model.Page;
using ProjectDataCore.Data.Structures.Page;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Page;

/// <summary>
/// Handles creation, editing, and removal of custom
/// pages to be used by the custom router.
/// </summary>
public interface IPageEditService
{
    #region Page Actions
    /// <summary>
    /// Creates a new website page.
    /// </summary>
    /// <param name="name">The name of the page.</param>
    /// <param name="route">The route to access the page at.</param>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> CreateNewPageAsync(string name, string route);

    /// <summary>
    /// Update an existing page.
    /// </summary>
    /// <param name="page">The ID of the page to update.</param>
    /// <param name="action">The action to perform on the page.</param>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdatePageAsync(Guid page, Action<CustomPageSettingsEditModel> action);

    /// <summary>
    /// Sets a page's primary layout.
    /// </summary>
    /// <remarks>
    /// This operation will create a new layout object,
    /// orphaning any other objects that were already there.
    /// 
    /// It is a distructive operation that will delete the old
    /// layout for the provided page, and all of its children.
    /// </remarks>
    /// <param name="page">The page to set a layout for.</param>
    /// <param name="layout">The <see cref="Type"/> of the layout to use.</param>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> SetPageLayoutAsync(Guid page, Type layout);

    /// <summary>
    /// Delete an existing page.
    /// </summary>
    /// <param name="page">The ID of the page to delete.</param>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeletePageAsync(Guid page);

    /// <summary>
    /// Get a single page by its <see cref="Guid"/>
    /// </summary>
    /// <remarks>
    /// This operation will load the entire page settings object, including all
    /// of its child objects.
    /// </remarks>
    /// <param name="page">The ID of the page to retrieve.</param>
    /// <returns>A task with an <see cref="ActionResult"/> that contains a 
    /// <see cref="CustomPageSettings"/> for the provided ID.</returns>
    public Task<ActionResult<CustomPageSettings>> GetPageSettingsAsync(Guid page);

    /// <summary>
    /// Get all custom website page setting objects.
    /// </summary>
    /// <returns>A task that contains a list 
    /// of all <see cref="CustomPageSettings"/>.</returns>
    public Task<List<CustomPageSettings>> GetAllPagesAsync();
    #endregion

    #region Layout Component Actions
    /// <summary>
    /// Set a child value for a layout component. Overwrites any component that has
    /// the same position value.
    /// </summary>
    /// <param name="layout">The ID of the layout to add a child to.</param>
    /// <param name="component">The type of component to add.</param>
    /// <param name="position">The position value for the new component.</param>
    /// <returns>A task that returns <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> SetLayoutChildAsync(Guid layout, Type component, int position);
    #endregion

    #region Editable Component Actions

    #endregion

    #region Display Component Actions

    #endregion
}
