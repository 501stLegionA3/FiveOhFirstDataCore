﻿using ProjectDataCore.Data.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;

public interface IUserService
{
    public Task<DataCoreUser?> GetUserFromClaimsPrinciaplAsync(ClaimsPrincipal claims);
    public Task<DataCoreUser?> GetUserFromIdAsync(int id);
}