using ProjectDataCore.Data.Structures.Model.Page;
using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Policy;

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
    /// Updates the page with a new authoriaztion scheme.
    /// </summary>
    /// <param name="key">The page to update.</param>
    /// <param name="newAuthorzationItem">The new authoriaztion policy to use.</param>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdatePermissionsAsync(Guid key, bool requireAuth, DynamicAuthorizationPolicy newAuthorzationItem);

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

    #region Global Component Actions
    /// <summary>
    /// Updates the display name of a component.
    /// </summary>
    /// <param name="key">The key of the component to update.</param>
    /// <param name="componentName">The new display name for the component.</param>
    /// <returns>A task that returns an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateDisplayNameAsync(Guid key, string componentName);
    /// <summary>
    /// Gets all avalible useres scopes for the currently loaded instance of the site.
    /// </summary>
    /// <returns>A task that returns an <see cref="ActionResult"/> with a <see cref="List{T}"/> of <see cref="PageComponentSettingsBase"/>
    /// that represent the avalible scopes for the pages at time of the method being called.</returns>
    public Task<ActionResult<List<PageComponentSettingsBase>>> GetAvalibleScopesAsync();
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
    /// <summary>
    /// Deletes a layout component and all of its children.
    /// </summary>
    /// <param name="layout">The layout to remove.</param>
    /// <returns>A task that returns <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteLayoutComponentAsync(Guid layout);
    #endregion

    #region Editable Component Actions
    /// <summary>
    /// Deletes an editable component.
    /// </summary>
    /// <param name="comp">The component to delete.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteEditableComponentAsync(Guid comp);
    /// <summary>
    /// Update an Editable component.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="action">The update action to take.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateEditableComponentAsync(Guid comp, Action<EditableComponentSettingsEditModel> action);
    #endregion

    #region Display Component Actions
    /// <summary>
    /// Deletes a display component.
    /// </summary>
    /// <param name="comp">The component to delete.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteDisplayComponentAsync(Guid comp);
    /// <summary>
    /// Update a display component.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="action">The update action to take.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateDisplayComponentAsync(Guid comp, Action<ParameterComponentSettingsEditModel> action);
    #endregion

    #region Roster Component Actions
    /// <summary>
    /// Deletes a roster component.
    /// </summary>
    /// <param name="comp">The component to delete.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteRosterComponentAsync(Guid comp);
    /// <summary>
    /// Update a roster component.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="action">The update action to take.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateRosterComponentAsync(Guid comp, Action<RosterComponentSettingsEditModel> action);
    #endregion

    #region Button Component Actions
    /// <summary>
    /// Deletes a roster component.
    /// </summary>
    /// <param name="comp">The component to delete.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteButtonComponentAsync(Guid comp);
    /// <summary>
    /// Update a roster component.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="action">The update action to take.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateButtonComponentAsync(Guid comp, Action<ButtonComponentSettingsEditModel> action);
    #endregion

    #region Text Display Component Actions
    /// <summary>
    /// Deletes a text display component.
    /// </summary>
    /// <param name="comp">The component to delete.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> DeleteTextDisplayComponentAsync(Guid comp);
    /// <summary>
    /// Update a text display component.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="action">The update action to take.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateTextDisplayComponentAsync(Guid comp, Action<TextDisplayComponentSettingsEditModel> action);
    /// <summary>
    /// Updates the raw display contents of a text display.
    /// </summary>
    /// <param name="comp">The ID of the component to update.</param>
    /// <param name="rawContents">The raw contnets to update.</param>
    /// <returns>A task that returns a <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UpdateTextDisplayContentsAsync(Guid comp, string rawContents);
    #endregion
}
