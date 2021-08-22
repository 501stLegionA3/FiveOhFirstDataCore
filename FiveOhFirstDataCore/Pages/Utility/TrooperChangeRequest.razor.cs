using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Utility
{
    public partial class TrooperChangeRequest
    {
        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        public TrooperChangeRequestData Model { get; set; } = new();

        public List<string> Errors { get; set; } = new();
        public string? SuccessMessage { get; set; } = null;

        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IEparService Epar { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task OnSubmit()
        {
            ClearErrors();
            ClearSuccess();

            if (Model.Reason is null)
            {
                Errors.Add("You must include a reason in your request.");
            }

            if(!Model.HasChange())
            {
                Errors.Add("You must have at least one change in your request.");
            }

            if(CurrentUser is null)
            {
                Errors.Add("You must be signed in.");
            }

            if (Errors.Count > 0) return;

            var res = await Epar.CreateChangeRequest(Model, CurrentUser!.Id);

            if (!res.GetResult(out var err))
            {
                Errors.AddRange(err);
            }
            else
            {
                Model = new();
                SuccessMessage = "Change request has been submitted.";
            }
        }

        private void ClearErrors()
        {
            Errors.Clear();
        }

        private void ClearSuccess()
        {
            SuccessMessage = null;
        }
    }
}
