using FiveOhFirstDataCore.Core.Data.Import;
using FiveOhFirstDataCore.Core.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    /// <summary>
    /// The <see cref="IImportService" /> contains methods for importing 501st roster data that has been
    /// exported as CSV files.
    /// </summary>
    public interface IImportService
    {
        /// <summary>
        /// Verifys the unsafe import folder for the application is created. if it is not, it will create the folder.
        /// </summary>
        public void VerifyUnsafeFolder();
        /// <summary>
        /// Import ORBAT data for the updating and/or creation of new accounts.
        /// </summary>
        /// <param name="import">Data to import.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        public Task<ImportResult> ImportOrbatDataAsync(OrbatImport import);
        /// <summary>
        /// Import Supporting Elemnts data for updating and/or creating new accounts.
        /// </summary>
        /// <param name="import">Data to import.</param>
        /// <returns>A <see cref="Task"/> with the <see cref="ImportResult"/> for this action.</returns>
        public Task<ImportResult> ImportSupportingElementsDataAsync(SupportingElementsImport import);
    }
}
