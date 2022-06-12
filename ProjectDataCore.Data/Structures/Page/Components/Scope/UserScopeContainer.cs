using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Scope;
public class UserScopeProviderContainer : DataObject<Guid>
{
    public UserScope ListeningScope { get; set; }
	public Guid ListeningScopeId { get; set; }
    public PageComponentSettingsBase ProvidingComponent { get; set; }
    public Guid ProvidingComponentId { get; set; }

    public int Order { get; set; }
}

public class UserScopeListenerContainer : DataObject<Guid>
{
    public UserScope ProvidingScope { get; set; }
    public Guid ProvidingScopeId { get; set; }
    public PageComponentSettingsBase ListeningComponent { get; set; }
    public Guid ListeningComponentId { get; set; }

    public int Order { get; set; }
}
