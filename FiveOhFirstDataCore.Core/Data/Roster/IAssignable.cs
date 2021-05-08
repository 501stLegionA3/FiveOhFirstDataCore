using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Roster
{
    public interface IAssignable<T>
    {
        public void Assign(T item);
    }
}
