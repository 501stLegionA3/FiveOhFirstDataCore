using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Message
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
        [Inject]
        public IAlertService AlertService { get; set; }

        public string NewMessage { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
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
            List<string> errors = new();
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                var res = await MessageService.PostMessageAsync(new()
                {
                    AuthorId = CurrentUser!.Id,
                    Message = NewMessage,
                    MessageFor = BoardFor
                });

                if (!res.GetResult(out var err))
                    errors.AddRange(err);
                else
                {
                    AlertService.PostAlert(this, "Message posted.");
                    await base.SetPage(base.Segments);
                    if (OnAfterMessagePostAsync is not null)
                        await OnAfterMessagePostAsync.Invoke();
                }
            }
            else
            {
                errors.Add("The message can not be empty!");
            }

            if (errors.Count > 0)
                AlertService.PostAlert(this, errors);
        }
    }
}
