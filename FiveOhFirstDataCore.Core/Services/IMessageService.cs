using FiveOhFirstDataCore.Data.Components.Base;
using FiveOhFirstDataCore.Data.Structures.Message;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Post a new message for this service.
        /// </summary>
        /// <remarks>
        /// The <see cref="TrooperMessage"/> object requires the AuthorId, MessageFor, and Message fields
        /// to have values. All other values will be generated automatically.
        /// </remarks>
        /// <param name="message">The <see cref="TrooperMessage"/> to post.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> PostMessageAsync(TrooperMessage message);

        /// <summary>
        /// Get trooper messages.
        /// </summary>
        /// <remarks>
        /// This method is desigend to be used with the <see cref="PaginationModel"/> and registered
        /// to the model during initalization. This should not be used on its own.
        /// <br /><br />
        /// There is an optional second and argument for the args list, an <see cref="int"/> value
        /// that is a trooper id and, if set, will update that troopers notification tracker to the date
        /// of the last retrived message.
        /// </remarks>
        /// <param name="start">The starting message.</param>
        /// <param name="end">The ending message.</param>
        /// <param name="args">Arguments. Requires args[0] to be a GUID for the object the messages are pulled from.</param>
        /// <returns>A task the returns a <see cref="IReadOnlyList{T}"/> of type <see cref="TrooperMessage"/>
        /// that contains the requested messages.</returns>
        public Task<IReadOnlyList<TrooperMessage>> GetTrooperMessagesAsync(int start, int end, object[]? args = null);
        /// <summary>
        /// Gets total count of itmes that can be retrived.
        /// </summary>
        /// <remarks>
        /// This method is desigend to be used with the <see cref="PaginationModel"/> and registered
        /// to the model during initalization. This should not be used on its own.
        /// </remarks>
        /// <param name="args">Arguments. Requires args[0] to be a GUID for the object the messages are pulled from.</param>
        /// <returns>A task that returns an <see cref="int"/> that equals the total number of messages.</returns>
        public Task<int> GetTrooperMessageCountsAsync(object[]? args = null);
    }
}
