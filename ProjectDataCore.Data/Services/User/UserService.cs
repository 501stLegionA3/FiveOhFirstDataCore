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

	public async Task<DataCoreUser?> GetUserFromClaimsPrinciaplAsync(ClaimsPrincipal claims)
	{
		_ = int.TryParse(_userManager.GetUserId(claims), out int id);
		return await GetUserFromIdAsync(id);
	}

	public async Task<DataCoreUser?> GetUserFromIdAsync(int id)
	{
		await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

		var user = await _dbContext.FindAsync<DataCoreUser>(id);

		return user;
	}
}
