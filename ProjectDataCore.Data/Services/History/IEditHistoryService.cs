using ProjectDataCore.Data.Structures.History;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.History;
public interface IEditHistoryService
{
    public Task<ActionResult> UndoAsync();
    public Task<ActionResult> RedoAsync();
    public void Push(EditHistoryItemBase item);
}
