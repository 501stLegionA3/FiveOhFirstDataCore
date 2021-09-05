using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Message;
using FiveOhFirstDataCore.Core.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Pages.Message
{
    public partial class TrooperMessageBoard
    {
        [Parameter]
        public Guid BoardFor { get; set; }
        [Parameter]
        public int AnonId { get; set; } = 0;
        [Parameter]
        public Func<Task> OnAfterMessagePostAsync { get; set; }
        [Parameter]
        public bool UpdateNotificationTrackers { get; set; } = true;

        [CascadingParameter]
        public Trooper? CurrentUser { get; set; }

        [Inject]
        public IMessageService MessageService { get; set; }

        public string NewMessage { get; set; }

        private List<string> Errors { get; set; } = new();
        private string? Success { get; set; } = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                object[] args;
                if (UpdateNotificationTrackers)
                    args = new object[] { BoardFor, CurrentUser?.Id ?? 0 };
                else
                    args = new object[] { BoardFor };

                await base.InitalizeAsync(MessageService.GetTrooperMessagesAsync,
                    MessageService.GetTrooperMessageCountsAsync, args);
                StateHasChanged();
            }
        }

        protected async Task PostMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                var res = await MessageService.PostMessageAsync(new()
                {
                    AuthorId = CurrentUser!.Id,
                    Message = NewMessage,
                    MessageFor = BoardFor
                });

                if (!res.GetResult(out var err))
                    Errors.AddRange(err);
                else
                {
                    Success = "Message posted.";
                    await base.SetPage(base.Segments);
                    if (OnAfterMessagePostAsync is not null)
                        await OnAfterMessagePostAsync.Invoke();
                }
            }
            else
            {
                Errors.Add("The message can not be empty!");
            }
        }

        protected void ClearErrors()
        {
            Errors.Clear();
        }

        protected void ClearSuccess()
        {
            Success = null;
        }
    }
}
