using FiveOhFirstDataCore.Data.Structuresbase;

using Lucene.Net.Analysis.Phonetic.Language;

using Microsoft.EntityFrameworkCore;

using System.Collections.Concurrent;

namespace FiveOhFirstDataCore.Data.Services
{
    public class NicknameComparisionService : INicknameComparisonService
    {
        private ConcurrentDictionary<string, HashSet<string>> Keys { get; set; }

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public NicknameComparisionService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            Keys = new();
        }

        public async Task InitializeAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            Keys = new();
            var encoder = new DoubleMetaphone();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                var key = encoder.GetDoubleMetaphone(x.NickName);

                if (key is null) return;

                if (Keys.TryGetValue(key, out var list))
                {
                    list.Add(x.NickName);
                }
                else
                {
                    Keys[key] = new() { x.NickName };
                }
            });
        }

        public Task<List<string>> GetPhoneticMatches(string nickname)
        {
            var meta = new DoubleMetaphone();
            var toMatch = meta.GetDoubleMetaphone(nickname);
            List<string> names = new();
            foreach (var pair in Keys)
            {
                if (meta.IsDoubleMetaphoneEqual(pair.Key, toMatch))
                {
                    names.AddRange(pair.Value);
                }
            }

            return Task.FromResult(names);
        }
    }
}
