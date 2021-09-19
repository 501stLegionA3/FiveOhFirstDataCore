using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Notice;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface INoticeService
    {
        /// <summary>
        /// Get or create a new notice board for this name.
        /// </summary>
        /// <param name="name">The name of the notice board.</param>
        /// <returns>A <see cref="NoticeBoardData"/> with a list of <see cref="Notice"/> inside.</returns>
        public Task<NoticeBoardData?> GetOrCreateNoticeBoardAsync(string name);
        /// <summary>
        /// Saves a new notice to the given board.
        /// </summary>
        /// <param name="newNotice">The new <see cref="Notice"/> to save.</param>
        /// <param name="board">The board name</param>
        /// <param name="user">The user who is posting the new notice.</param>
        /// <returns>The <see cref="Task"/> representing this operation.</returns>
        public Task PostNoticeAsync(Notice newNotice, string board, Trooper user);
        /// <summary>
        /// Deletes a given notice.
        /// </summary>
        /// <param name="toRemove">The <see cref="Notice"/> to remove.</param>
        /// <returns>A <see cref="Task"/> representing this operation.</returns>
        public Task DeleteNoticeAsync(Notice toRemove);
        /// <summary>
        /// Determines if this person is one of the provided allowed roles in the selected C-Shops.
        /// </summary>
        /// <param name="claims"><see cref="ClaimsPrincipal"/> for the signed in user.</param>
        /// <param name="cshops">The <see cref="CShop"/> to check agains.</param>
        /// <param name="allowed">A list of grant values needed to be allowed to edit the notices.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="bool"/> that determines if they can edit notices.</returns>
        public Task<bool> IsAllowedCShopEditor(ClaimsPrincipal claims, CShop cshops, List<string> allowed);
        /// <summary>
        /// Saves the changes made to a notice.
        /// </summary>
        /// <param name="toSave">The notice to save.</param>
        /// <returns>A <see cref="Task"/> representing this operation.</returns>
        public Task SaveChangesAsync(Notice toSave);
    }
}
