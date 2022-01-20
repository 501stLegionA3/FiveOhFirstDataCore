using Microsoft.AspNetCore.Identity;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Model.User;

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

    public async Task<ActionResult> UpdateUserAsync(Guid user, Action<DataCoreUserEditModel> action)
    {
		await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

		var userData = await _dbContext.FindAsync<DataCoreUser>(user);

		if (userData is null)
			return new(false, new List<string>() { "No user was found for the provided ID." });

		DataCoreUserEditModel model = new();
		action.Invoke(model);

		try
		{
			model.ApplyStaticValues(userData);
		}
		catch (Exception ex)
        {
			return new(false, new List<string>() { "Failed to apply static value updates.", ex.Message });
        }

		foreach (var item in model.AssignableValues)
        {
			var property = userData.AssignableValues
				.Find(x => x.AssignableConfiguration.PropertyName == item.Key);

			if(property is not null && item.Value is not null)
				property.ReplaceValue(item.Value);
        }

		await _dbContext.SaveChangesAsync();

		return new(true, null);
    }
}
