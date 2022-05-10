using ProjectDataCore.Data.Structures.History;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.History;
public interface IEditHistoryService
{
    /// <summary>
    /// Resets the services to its default state.
    /// </summary>
    public void Reset();
    /// <summary>
    /// Perform an undo action if avalible.
    /// </summary>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> UndoAsync();
    /// <summary>
    /// Perform a redo action if avalible.
    /// </summary>
    /// <returns>A task with an <see cref="ActionResult"/> for this action.</returns>
    public Task<ActionResult> RedoAsync();
    /// <summary>
    /// Push an <see cref="EditHistoryItemBase"/> to the undo stack.
    /// </summary>
    /// <param name="item">The item to push to the undo stack.</param>
    public void Push(EditHistoryItemBase item);
}
