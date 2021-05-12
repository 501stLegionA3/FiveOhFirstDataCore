using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class RegisterTrooperResult
    {
        private bool Success { get; set; }
        public string? AccessToken { get; set; }
        private List<string>? Errors { get; set; }

        public RegisterTrooperResult(bool success, string? accessToken, List<string>? errors = null)
        {
            Success = success;
            Errors = errors;
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
