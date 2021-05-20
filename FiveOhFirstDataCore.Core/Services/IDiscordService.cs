using FiveOhFirstDataCore.Core.Structures.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IDiscordService
    {
        public Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor);
        public Task UpdateQualificationChangeAsync(QualificationChange change, ulong changeFor);
        public Task UpdateRankChangeAsync(RankChange change, ulong changeFor);
        public Task UpdateSlotChangeAsync(SlotChange change, ulong changeFor);
    }
}
