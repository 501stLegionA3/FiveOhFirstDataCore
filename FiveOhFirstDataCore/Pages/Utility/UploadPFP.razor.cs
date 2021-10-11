using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Notice;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

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
        [Inject]
        public IAlertService Alert { get; set; }

        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }
        public int BytesRead { get; set; }
        public byte[] pfp;

        private async Task Upload(InputFileChangeEventArgs e)
        {
            if (e.GetMultipleFiles(1) is not null && e.FileCount > 0 && CurrentUser is not null)
            {
                try
                {
                    BytesRead = 0;
                    await using var pfpStream = e.GetMultipleFiles(1)[0].OpenReadStream(2000 * 1024); //max pfp size 256kb
                    pfp = new byte[pfpStream.Length];
                    while (BytesRead < pfp.Length)
                    {
                        BytesRead += await pfpStream.ReadAsync(pfp, BytesRead, pfp.Length - BytesRead);
                    }
                    await _upload.UploadPFP(CurrentUser, pfp);

                    Alert.PostAlert(this, new AlertData()
                    {
                        Level = AlertLevel.Success,
                        Content = $"Profile image has been updated!"
                    });
                }
                catch
                {
                    Alert.PostAlert(this, new AlertData()
                    {
                        Level = AlertLevel.Danger,
                        Content = $"File was too large. Please upload an image less than 2 MB"
                    });
                }
            }
        }
    }
}
