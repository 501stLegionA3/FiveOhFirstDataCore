using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Assignable.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class BaseAssignableValue : DataObject<Guid>
{
    public DataCoreUser ForUser { get; set; }
    public Guid ForUserId { get; set; }

    public BaseAssignableConfiguration AssignableConfiguration { get; set; }
    public Guid AssignableConfigurationId { get; set; }
}
