using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Structures
{
    /// <summary>
    /// Holds details for results of import operations. Mainly used in <see cref="FiveOhFirstDataCore.Core.Services.IImportService"/>.
    /// </summary>
    public class ImportResult : ResultBase
    {
        protected List<string>? Warnings { get; set; }

        public ImportResult(bool success, List<string>? errors = null, List<string>? warnings = null)
            : base(success, errors)
        {
            Warnings = warnings;
        }

        public virtual bool GetResultWithWarnings([MaybeNullWhen(true), NotNullWhen(false)] out List<string>? warnings,
            [NotNullWhen(false)] out List<string>? errors)
        {
            var res = base.GetResult(out errors);

            if (res)
            {
                warnings = Warnings;
                return res;
            }
            else
            {
                warnings = Warnings ?? new();
                return res;
            }
        }
    }
}
