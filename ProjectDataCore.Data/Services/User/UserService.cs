using Microsoft.AspNetCore.Identity;

using ProjectDataCore.Data.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly UserManager<DataCoreUser> _userManager;

	public UserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<DataCoreUser> userManager)
		=> (_dbContextFactory, _userManager) = (dbContextFactory, userManager);

    public async Task<List<DataCoreUser>> GetAllUsersAsync()
    {
		await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

		return await _dbContext.Users.ToListAsync();
    }

    public async Task<DataCoreUser?> GetUserFromClaimsPrinciaplAsync(ClaimsPrincipal claims)
	{
		_ = Guid.TryParse(_userManager.GetUserId(claims), out Guid id);
		return await GetUserFromIdAsync(id);
	}

	public async Task<DataCoreUser?> GetUserFromIdAsync(Guid id)
	{
		await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

		var user = await _dbContext.FindAsync<DataCoreUser>(id);

		return user;
	}
}
