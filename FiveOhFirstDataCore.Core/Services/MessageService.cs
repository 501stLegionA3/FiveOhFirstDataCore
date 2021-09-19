using FiveOhFirstDataCore.Data.Structures.Message;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.EntityFrameworkCore;

namespace FiveOhFirstDataCore.Data.Services
{
    public class MessageService : IMessageService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly INotificationService _notification;

        public MessageService(IDbContextFactory<ApplicationDbContext> dbContextFactory,
            INotificationService notification)
            => (_dbContextFactory, _notification) = (dbContextFactory, notification);

        public async Task<int> GetTrooperMessageCountsAsync(object[]? args = null)
        {
            var id = args?.GetArgument<Guid>(0) ?? null;
            if (id is null) return 0;

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.TrooperMessages
                .Where(x => x.MessageFor == id.Value)
                .CountAsync();
        }

        public async Task<IReadOnlyList<TrooperMessage>> GetTrooperMessagesAsync(int start, int end, object[]? args = null)
        {
            var id = args?.GetArgument<Guid>(0) ?? null;
            if (id is null) return Array.Empty<TrooperMessage>();

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var set = _dbContext.TrooperMessages
                .Where(x => x.MessageFor == id.Value)
                .Include(x => x.Author)
                .OrderBy(x => x.CreatedOn)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();

            if (args!.Length > 1)
            {
                var userId = args.GetArgument<int>(1);
                if (userId != default)
                {
                    _ = Task.Run(async ()
                        => await _notification.UpdateReportViewDateTimeAsync(id.Value, userId, set[set.Count - 1].CreatedOn));
                }
            }

            return set;
        }

        public async Task<ResultBase> PostMessageAsync(TrooperMessage message)
        {
            List<string> errors = new();
            if (message.AuthorId == default)
                errors.Add($"{nameof(message.AuthorId)} must have a value.");
            if (message.MessageFor == default)
                errors.Add($"{nameof(message.MessageFor)} must have a value.");
            if (string.IsNullOrWhiteSpace(message.Message))
                errors.Add($"{nameof(message.Message)} must have a non-whitespace value.");

            if (errors.Count > 0)
                return new(false, errors);

            message.CreatedOn = DateTime.UtcNow;

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            _dbContext.TrooperMessages.Add(message);

            var edits = await _dbContext.SaveChangesAsync();
            if (edits > 0)
                return new(true, null);
            else return new(false, new() { "Internal DB error: no changes were written to the database." });
        }
    }
}
