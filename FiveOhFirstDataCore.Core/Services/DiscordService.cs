using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class DiscordService : IDiscordService
    {
        private readonly DiscordRestClient _rest;

        public DiscordService(DiscordRestClient rest)
        {
            _rest = rest;
        }


    }
}
