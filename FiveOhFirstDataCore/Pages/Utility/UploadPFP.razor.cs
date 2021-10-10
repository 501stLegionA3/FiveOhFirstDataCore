using FiveOhFirstDataCore.Data.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class UploadPFP
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }
        public int BytesRead { get; set; }
        public byte[] pfp;

        private async Task Upload(InputFileChangeEventArgs e)
        {
            if (e.GetMultipleFiles(1) is not null && e.FileCount > 0 && CurrentUser is not null)
            {
                await using var pfpStream = e.GetMultipleFiles(1)[0].OpenReadStream(256 * 1024); //max pfp size 256kb
                pfp = new byte[pfpStream.Length];
                BytesRead = await pfpStream.ReadAsync(pfp, 0, (int)pfp.Length-1);
                await _upload.UploadPFP(CurrentUser, pfp);
            }
        }
    }
}
