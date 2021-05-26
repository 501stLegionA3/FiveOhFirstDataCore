using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface INoticeService
    {
        /// <summary>
        /// Get or create a new notice board for this name.
        /// </summary>
        /// <param name="name">The name of the notice board.</param>
        /// <returns>A <see cref="NoticeBoardData"/> with a list of <see cref="Notice"/> inside.</returns>
        public Task<NoticeBoardData> GetOrCreateNoticeBoardAsync(string name);
        /// <summary>
        /// Saves a new notice to the given board.
        /// </summary>
        /// <param name="newNotice">The new <see cref="Notice"/> to save.</param>
        /// <param name="board">The board name</param>
        /// <param name="user">The user who is posting the new notice.</param>
        /// <returns>The <see cref="Task"/> reprsenting this operation.</returns>
        public Task PostNoticeAsync(Notice newNotice, string board, Trooper user);
        /// <summary>
        /// Deletes a notice from the given board.
        /// </summary>
        /// <param name="toRemove">The <see cref="Notice"/> to remove.</param>
        /// <param name="board">The board name.</param>
        /// <returns>A <see cref="Task"/> representing this operation.</returns>
        public Task DelteNoticeAsync(Notice toRemove, string board);
        /// <summary>
        /// Initates a database save operation.
        /// </summary>
        /// <returns></returns>
        public Task SaveChangesAsync();
    }
}
