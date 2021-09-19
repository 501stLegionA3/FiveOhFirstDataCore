using FiveOhFirstDataCore.Data.Structures.Import;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Services
{
    /// <summary>
    /// The <see cref="IImportService" /> contains methods for importing 501st roster data that has been
    /// exported as CSV files.
    /// </summary>
    public interface IImportService
    {
        /// <summary>
        /// Verifies the unsafe import folder for the application is created. if it is not, it will create the folder.
        /// </summary>
        public void VerifyUnsafeFolder();
        /// <summary>
        /// Import ORBAT data for the updating and/or creation of new accounts.
        /// </summary>
        /// <param name="import">Data to import.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        public Task<ImportResult> ImportOrbatDataAsync(OrbatImport import);
        /// <summary>
        /// Import Supporting Elements data for updating and/or creating new accounts.
        /// </summary>
        /// <param name="import">Data to import.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        public Task<ImportResult> ImportSupportingElementsDataAsync(SupportingElementsImport import);
        /// <summary>
        /// Import Qualification data for updating existing users qualification data.
        /// </summary>
        /// <param name="import">The <see cref="QualificationImport"/> class that holds qualification data.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        public Task<ImportResult> ImportQualificationDataAsync(QualificationImport import);
    }
}
