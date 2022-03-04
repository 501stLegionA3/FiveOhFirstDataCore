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

    public virtual object? GetValue()
    {
        return null;
    }

    public virtual List<object?> GetValues()
    {
        return Array.Empty<object?>().ToList();
    }

    public virtual void ReplaceValue(object value, int? index = null)
    {
        // Do nothing.
    }

    public virtual void AddValue(object value)
    {
        // Do nothing.
    }

    public virtual void ConvertAndReplaceValue(object value, int? index = null)
    {
        // Do nothing.
    }

    public virtual void ConvertAndAddValue(object value)
    {
        // Do nothing.
    }

    public virtual void ClearValue()
    {
        // Do nothing.
    }

    public virtual BaseAssignableValue Clone()
    {
        return this;
    }
}
