using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.HTML;
public interface IHTMLService
{
    public Task<ActionResult> UpdateCustomSiteCSSAsync(string? additionalHtml = null);
}
