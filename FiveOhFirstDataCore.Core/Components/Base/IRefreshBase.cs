using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components.Base
{
    public interface IRefreshBase : IDisposable
    {
        public Task CallRefreshRequest();
    }
}
