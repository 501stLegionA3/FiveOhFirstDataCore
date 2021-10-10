using FiveOhFirstDataCore.Data.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IUploadService
    {
        /// <summary>
        /// Uploads PFP binary data for a specific trooper directly to the Database.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> Object the PFP Belongs too.</param>
        /// <param name="pfp">The PFP Binary Data</param>
        /// <example>CurrentUser.PFP = new byte[pfp.Length]; 
        /// pfp.Read(CurrentUser.PFP, 0, (int)pfp.Length-1);
        /// </example>
        /// <returns></returns>
        public Task UploadPFP(Trooper trooper, byte[] pfp);
    }
}
