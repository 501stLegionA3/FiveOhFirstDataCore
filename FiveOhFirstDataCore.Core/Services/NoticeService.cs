﻿using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Notice;
using FiveOhFirstDataCore.Core.Database;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
        private readonly ApplicationDbContext _dbContext;

        public NoticeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DelteNoticeAsync(Notice toRemove, string board)
        {
            var item = await _dbContext.FindAsync<NoticeBoardData>(board);

            if (item is null) item = await GetOrCreateNoticeBoardAsync(board);

            item.Notices.Remove(toRemove);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<NoticeBoardData> GetOrCreateNoticeBoardAsync(string name)
        {
            var board = await _dbContext.FindAsync<NoticeBoardData>(name);

            if(board is null)
            {
                board = new()
                {
                    Loaction = name
                };

                await _dbContext.AddAsync(board);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.Entry(board).Collection(e => e.Notices).LoadAsync();

            return board;
        }

        public Task<bool> IsAllowedCShopEditor(ClaimsPrincipal claims, CShop cshops, List<string> allowed)
        {
            foreach(CShop shop in Enum.GetValues(typeof(CShop)))
            {
                if((shop & cshops) == shop)
                {
                    foreach(var groupName in CShopExtensions.ClaimsTree[shop])
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
            var item = await _dbContext.FindAsync<NoticeBoardData>(board);

            if (item is null) item = await GetOrCreateNoticeBoardAsync(board);

            newNotice.AuthorId = user.Id;
            newNotice.PostedOn = DateTime.UtcNow;

            item.Notices.Add(newNotice);

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() 
            => await _dbContext.SaveChangesAsync();
    }
}