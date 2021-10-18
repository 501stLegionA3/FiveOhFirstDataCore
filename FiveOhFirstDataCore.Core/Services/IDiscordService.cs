using DSharpPlus.Entities;

using FiveOhFirstDataCore.Data.Structures.Updates;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IDiscordService
    {
        /// <summary>
        /// Initialize the Discord Service
        /// </summary>
        /// <returns>A task for this action.</returns>
        public Task InitalizeAsync();
        /// <summary>
        /// Update the C-Shop for a user on Discord.
        /// </summary>
        /// <param name="add">A list of <see cref="Claim"/> data that holds claims that were added to a user.</param>
        /// <param name="remove">A list of <see cref="Claim"/> data that holds claims that were removed from a user.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateCShopAsync(List<Claim> add, List<Claim> remove, ulong changeFor, int changeForId);
        /// <summary>
        /// Update the Qualifications for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="QualificationUpdate"/> object that contains the change data.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateQualificationChangeAsync(QualificationUpdate change, ulong changeFor, int changeForId);
        /// <summary>
        /// Update the rank for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="RankUpdate"/> with change details.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task" /> representing this action.</returns>
        public Task UpdateRankChangeAsync(RankUpdate change, ulong changeFor, int changeForId);
        /// <summary>
        /// Update the slot for a user on Discord.
        /// </summary>
        /// <param name="change">A <see cref="SlotUpdate"/> with the change details.</param>
        /// <param name="changeFor">The Discord ID of the user to modify.</param>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task UpdateSlotChangeAsync(SlotUpdate change, ulong changeFor, int changeForId);
        /// <summary>
        /// Gets all <see cref="DiscordRole"/> objects for the home guild.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/> of <see cref="DiscordRole"/> objects.</returns>
        public Task<IReadOnlyList<DiscordRole>> GetAllHomeGuildRolesAsync();
        /// <summary>
        /// Gets all <see cref="DiscordChannel"/> objects for the home guild.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/> of <see cref="DiscordChannel"/> objects.</returns>
        public Task<IReadOnlyList<DiscordChannel>> GetAllHomeGuildChannelsAsync();
    }
}
