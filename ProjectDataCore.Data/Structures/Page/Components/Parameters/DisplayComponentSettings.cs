
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Parameters;

/// <summary>
/// Displays a single data value.
/// </summary>
public class DisplayComponentSettings : ParameterComponentSettingsBase
{
    private MarkupString? _authorizedMarkup;
    public MarkupString AuthorizedMarkup 
    { 
        get
        {
            if (_authorizedMarkup is null)
                _authorizedMarkup = new(AuthorizedRaw);

            return _authorizedMarkup.Value;
        }        
    }

    public string AuthorizedRaw { get; set; }

    private MarkupString? _unauthorizedMarkup;
    public MarkupString UnAuthorizedMarkup
    {
        get
        {
            if (_unauthorizedMarkup is null)
                _unauthorizedMarkup = new(UnAuthorizedRaw);

            return _unauthorizedMarkup.Value;
        }
    }

    public string UnAuthorizedRaw { get; set; }
}
