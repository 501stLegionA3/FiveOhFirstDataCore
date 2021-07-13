using FiveOhFirstDataCore.Core.Data.Promotions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class PromotionResult : ResultBase
    {
        protected Promotion? Promotion { get; init; }

        public PromotionResult(bool succeded, Promotion? promotion, List<string>? errors = null)
            : base(succeded, errors)
        {
            Promotion = promotion;
        }

        public bool GetResult([NotNullWhen(true)] out Promotion? promotion,
            [NotNullWhen(false)] out List<string>? errors)
        {
            if(Success && Promotion is not null)
            {
                promotion = Promotion;
                errors = null;
                return true;
            }
            else
            {
                errors = Errors ?? new();
                promotion = null;
                return false;
            }
        }
    }
}
