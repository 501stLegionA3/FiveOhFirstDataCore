using Microsoft.AspNetCore.Identity;

using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.Roster;
using ProjectDataCore.Data.Structures.Model.User;
using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Util;

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
	private readonly IAssignableDataService _assignableDataService;
    private readonly UserManager<DataCoreUser> _userManager;
	private readonly IPolicyService _policyService;

	public UserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, 
		IAssignableDataService assignableDataService, UserManager<DataCoreUser> userManager,
		IPolicyService policyService)
		=> (_dbContextFactory, _assignableDataService, _userManager, _policyService)
		=  (dbContextFactory, assignableDataService, userManager, policyService);

    public async Task<bool> AuthorizeUserAsync(ClaimsPrincipal claims, PageComponentSettingsBase settings, bool forceReload = false)
	{

		if (settings.RequireAuth)
		{
			var user = await GetUserFromClaimsPrinciaplAsync(claims);

			if (user is null)
				return false;

			if(settings.AuthorizationPolicyKey is not null)
            {
				return await _policyService.AuthorizeAsync(user, settings.AuthorizationPolicyKey.Value, forceReload);
            }
        }

		return true;
    }

    public async Task<ActionResult> CreateOrUpdateAccountAsync(string? accessCode, string username, string password)
	{
		DataCoreUser user;
		if (accessCode is null)
		{
			user = new()
			{
				UserName = username,
			};

			var res = await _userManager.CreateAsync(user, password);

			if (!res.Succeeded)
				return new(false, res.Errors.ToList().ToList(x => x.Description));

			user = await _userManager.FindByNameAsync(user.UserName);

			await _assignableDataService.EnsureAssignableValuesAsync(user);

			return new(true, null);
		}
		else
        {
			user = await _userManager.FindByNameAsync(username);

			if (user is null || user.AccessCode != accessCode)
				return new(false, new List<string>() { "Access codes did not match." });

			var res = await _userManager.RemovePasswordAsync(user);
			if (!res.Succeeded)
				return new(false, res.Errors.ToList().ToList(x => x.Description));


			res = await _userManager.AddPasswordAsync(user, password);
			if (!res.Succeeded)
				return new(false, res.Errors.ToList().ToList(x => x.Description));


			res = await _userManager.SetUserNameAsync(user, username);
			if (!res.Succeeded)
				return new(false, res.Errors.ToList().ToList(x => x.Description));


			user.AccessCode = null;
			res = await _userManager.UpdateAsync(user);
			if (!res.Succeeded)
				return new(false, res.Errors.ToList().ToList(x => x.Description));


			return new(true, null);
		}
    }

    public async Task<ActionResult> CreateUserAsync(DataCoreUser user, Action<DataCoreUserEditModel> model)
    {
		var token = Guid.NewGuid().ToString();

		if(string.IsNullOrWhiteSpace(user.AccessCode))
			user.AccessCode = token;
		if(string.IsNullOrWhiteSpace(user.UserName))
			user.UserName = token;

		user.AssignableValues.Clear();

		var res = await _userManager.CreateAsync(user, user.AccessCode);

		if(res.Succeeded)
        {
			user = await _userManager.FindByNameAsync(user.UserName);

			var updateRes = await _assignableDataService.EnsureAssignableValuesAsync(user);

			if (updateRes.GetResult(out _))
			{
				updateRes = await UpdateUserAsync(user.Id, model);
			}

			return updateRes;
        }
		else
        {
			var err = new List<string>() { "Failed to create a new user account." };
			err.AddRange(res.Errors.ToList().ToList(x => x.Description));
			return new(false, err);
        }
    }

    public async Task<List<DataCoreUser>> GetAllUnregisteredUsersAsync()
    {
		await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

		return await _dbContext.Users
			.Where(x => x.AccessCode != null)
			.Include(x => x.AssignableValues)
			.ThenInclude(x => x.AssignableConfiguration)
			.ToListAsync();
	}

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

		if(model.Slots is not null)
		{
			foreach(var slot in model.Slots)
			{
				_dbContext.Attach(slot);

				slot.OccupiedById = userData.Id;
			}
		}

		await _dbContext.SaveChangesAsync();

		return new(true, null);
    }
}
