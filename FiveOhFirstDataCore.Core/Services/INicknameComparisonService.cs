using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface INicknameComparisonService
    {
        public Task InitializeAsync();
        public Task<List<string>> GetPhoneticMatches(string nickname);
    }
}
