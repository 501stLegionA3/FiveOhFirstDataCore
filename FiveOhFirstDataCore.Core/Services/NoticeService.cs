using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Notice;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class NoticeService : INoticeService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IWebsiteSettingsService _settings;

        public NoticeService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IWebsiteSettingsService settings)
        {
            _dbContextFactory = dbContextFactory;
            _settings = settings;
        }

        public async Task DeleteNoticeAsync(Notice toRemove)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            _dbContext.Remove(toRemove);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<NoticeBoardData?> GetOrCreateNoticeBoardAsync(string name)
        {
            try
            {
                using var _dbContext = _dbContextFactory.CreateDbContext();
                var board = await _dbContext.FindAsync<NoticeBoardData>(name);

                if (board is null)
                {
                    board = new()
                    {
                        Location = name
                    };

                    await _dbContext.AddAsync(board);
                    await _dbContext.SaveChangesAsync();
                }

                await _dbContext.Entry(board).Collection(e => e.Notices).LoadAsync();

                return board;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsAllowedCShopEditor(ClaimsPrincipal claims, CShop cshops, List<string> allowed)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            foreach (CShop shop in Enum.GetValues(typeof(CShop)))
            {
                if ((shop & cshops) == shop)
                {
                    if (ClaimsTree.TryGetValue(shop, out var data))
                    {
                        foreach (var groupName in data.ClaimData)
                        {
                            if (claims.HasClaim(x => x.Type.Equals(groupName.Key)
                                && groupName.Value.Contains(x.Value)
                                && allowed.Contains(x.Value)))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task PostNoticeAsync(Notice newNotice, string board, Trooper user)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var item = await _dbContext.FindAsync<NoticeBoardData>(board);

            if (item is null) item = await GetOrCreateNoticeBoardAsync(board);

            if (item is null) return;

            newNotice.AuthorId = user.Id;
            newNotice.PostedOn = DateTime.UtcNow.ToEst().ToEst();

            item.Notices.Add(newNotice);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(Notice toSave)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = _dbContext.Attach(toSave);
            _dbContext.Entry(toSave).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}