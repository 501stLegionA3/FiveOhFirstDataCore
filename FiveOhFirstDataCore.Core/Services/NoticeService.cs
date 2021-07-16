using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Notice;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class NoticeService : INoticeService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public NoticeService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task DeleteNoticeAsync(Notice toRemove, string board)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var item = await _dbContext.FindAsync<NoticeBoardData>(board);

            if (item is null) item = await GetOrCreateNoticeBoardAsync(board);

            if (item is null) return;

            item.Notices.Remove(toRemove);

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

        public Task<bool> IsAllowedCShopEditor(ClaimsPrincipal claims, CShop cshops, List<string> allowed)
        {
            foreach (CShop shop in Enum.GetValues(typeof(CShop)))
            {
                if ((shop & cshops) == shop)
                {
                    foreach (var groupName in CShopExtensions.ClaimsTree[shop])
                    {
                        if (claims.HasClaim(x => x.Type.Equals(groupName.Key)
                            && groupName.Value.Contains(x.Value)
                            && allowed.Contains(x.Value)))
                            return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
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