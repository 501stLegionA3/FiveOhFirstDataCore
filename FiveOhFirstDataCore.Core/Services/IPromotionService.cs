using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Structures;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IPromotionService
    {
        /// <summary>
        /// Start the promotion process with a skeleton promotion object.
        /// </summary>
        /// <remarks>
        /// The <paramref name="promotion"/> parameter requires a skeleton <see cref="Promotion"/> object
        /// that has at least the following properties with values:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="Promotion.PromotionFor"/></term>
        ///         <description>A <see cref="Trooper"/> represeting who the promotion is for.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Promotion.CurrentBoard"/></term>
        ///         <description>A <see cref="PromotionBoardLevel"/> that indicates the starting board of this promotion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Promotion.PromoteFrom"/></term>
        ///         <description>A <see cref="int"/> that represents the starting rank of this promotion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Promotion.PromoteTo"/></term>
        ///         <description>A <see cref="int"/> of a rank that indicates the goal of this promotion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Promotion.Reason"/></term>
        ///         <description>A <see cref="string"/> that details the reason behind this promotion.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Promotion.Forced"/></term>
        ///         <description>A <see cref="bool"/> that indicates if this promotion was forced or not.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="invoker">The <see cref="ClaimsPrincipal"/> of the <see cref="Trooper"/> who started the promotion.</param>
        /// <param name="promotion">A skeleton <see cref="Promotion" /> object.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="PromotionResult"/> for this promotion.</returns>
        public Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Promotion promotion);
        /// <summary>
        /// Start a promotion process.
        /// </summary>
        /// <param name="invoker"></param>
        /// <param name="promotionFor">A <see cref="Trooper"/> represeting who the promotion is for.</param>
        /// <param name="currentBoard">A <see cref="PromotionBoardLevel"/> that indicates the starting board of this promotion.</param>
        /// <param name="promotionFrom">A <see cref="int"/> that represents the starting rank of this promotion.</param>
        /// <param name="promotionTo">A <see cref="int"/> of a rank that indicates the goal of this promotion.</param>
        /// <param name="reason">A <see cref="string"/> that details the reason behind this promotion.</param>
        /// <param name="forced">A <see cref="bool"/> that indicates if this promotion was forced or not.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="PromotionResult"/> for this promotion.</returns>
        public Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Trooper promotionFor,
            PromotionBoardLevel currentBoard,
            int promotionFrom, int promotionTo, string reason, bool forced);
        /// <summary>
        /// Elevates a promotion to the next stage. Will finalize a promotion if able.
        /// </summary>
        /// <param name="promotion">Promotion to elevate.</param>
        /// <param name="approver">Trooper who approved the promotion.</param>
        /// <param name="levels">Ammount of levels to elevate the promotion by.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> ElevatePromotionAsync(Promotion promotion, Trooper approver, int levels = 1);
        /// <summary>
        /// Finalize a promotion.
        /// </summary>
        /// <remarks>
        /// This action will delete the promotion object and apply any changes for this promotion to the database.
        /// </remarks>
        /// <param name="promotion">The <see cref="Promotion"/> to fianlize.</param>
        /// <param name="approver">The <see cref="Trooper"/> who approved this action.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> FinalizePromotionAsync(Promotion promotion, Trooper approver);
        /// <summary>
        /// Cancel a promotion.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> to cancel.</param>
        /// <param name="denier">The <see cref="Trooper"/> who denied this promotion.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> CancelPromotionAsync(Promotion promotion, Trooper denier);

        /// <summary>
        /// Update the reason for a given promotion.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> to update.</param>
        /// <param name="reason">The new <see cref="Promotion.Reason"/> as a <see cref="string"/></param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> UpdatePromotionAsync(Promotion promotion, string reason);
    }
}
