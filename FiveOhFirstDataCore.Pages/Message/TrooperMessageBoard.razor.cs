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

        [Inject]
        public IMessageService MessageService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await base.InitalizeAsync(MessageService.GetTrooperMessagesAsync,
                    MessageService.GetTrooperMessageCountsAsync, new object[] { BoardFor });
                StateHasChanged();
            }
        }
    }
}
