using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Account;
public class AccountSettings : DataObject<Guid>
{
    /// <summary>
    /// Will a user require a link to a Discord account via OAuth2?
    /// </summary>
    public bool RequireDiscordLink { get; set; } = false;
    /// <summary>
    /// Will a user require a link to a Steam account via OAuth2?
    /// </summary>
    public bool RequireSteamLink { get; set; } = false;

    /// <summary>
    /// Does account registration require an access code?
    /// </summary>
    /// <remarks>
    /// This field is used to determine if users can create their own accounts. Settings this to
    /// true will require the use of a new user form display module to create new accounts.
    /// </remarks>
    public bool RequireAccessCodeForRegister { get; set; } = false;
}
