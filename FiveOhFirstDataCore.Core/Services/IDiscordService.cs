using FiveOhFirstDataCore.Core.Structures.Updates;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IDiscordService
    {
        public Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor);
        public Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor);
        public Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor);
        public Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor);
    }
}
