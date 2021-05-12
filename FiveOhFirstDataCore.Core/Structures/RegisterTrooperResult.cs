using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class RegisterTrooperResult : ResultBase
    {
        public string? AccessToken { get; set; }

        public RegisterTrooperResult(bool success, string? accessToken, List<string>? errors = null) : base(success, errors)
        {
            AccessToken = accessToken;
        }

        public bool GetResult([NotNullWhen(true)] out string? accessToken, [NotNullWhen(false)] out List<string>? errors)
        {
            if(Success)
            {
                errors = null;
                accessToken = AccessToken ?? "";
                return true;
            }
            else
            {
                errors = Errors ?? new();
                accessToken = null;
                return false;
            }
        }
    }
}
