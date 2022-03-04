using ProjectDataCore.Data.Structures.Util.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Import;

public interface IImportService
{
    public Task<ActionResult> BulkUpdateUsersAsync(DataImportConfiguration config, CancellationToken cancellationToken = default, Guid logScope = default);
    public Task<ActionResult> GetCSVUniqueValuesAsync(Stream dataStream, DataImportConfiguration config);
}
