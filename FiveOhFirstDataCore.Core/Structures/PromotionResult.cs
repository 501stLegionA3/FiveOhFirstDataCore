using FiveOhFirstDataCore.Data.Structures.Promotions;

using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Structures
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
            if (Success && Promotion is not null)
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
