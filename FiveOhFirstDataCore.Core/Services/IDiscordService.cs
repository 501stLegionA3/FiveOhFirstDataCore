using FiveOhFirstDataCore.Core.Structures.Updates;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IDiscordService
    {
        /// <summary>
        /// Update the C-Shop for a user on Discord.
        /// </summary>
        /// <param name="add">A list of <see cref="Claim"/> data that holds claims that were added to a user.</param>
        /// <param name="remove">A list of <see cref="Claim"/> data that holds claims that were removed from a user.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor);
        /// <summary>
        /// Update the Qualifications for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="QualificationUpdate"/> object that contains the change data.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor);
        /// <summary>
        /// Update the rank for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="RankUpdate"/> with change details.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor);
        /// <summary>
        /// Update the slot for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="SlotUpdate"/> with the change details.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor);
    }
}
