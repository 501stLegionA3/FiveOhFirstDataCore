namespace FiveOhFirstDataCore.Data.Services
{
    public interface INicknameComparisonService
    {
        /// <summary>
        /// Initalizes the nickname service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task InitializeAsync();
        /// <summary>
        /// Get the phonetic matches of a nickname.
        /// </summary>
        /// <param name="nickname">The nickname to compare to the current dataset.</param>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="string"/> where each <see cref="string"/> is a close match to the <paramref name="nickname"/></returns>
        public Task<List<string>> GetPhoneticMatches(string nickname);
    }
}
