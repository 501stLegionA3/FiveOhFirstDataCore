using FiveOhFirstDataCore.Data.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Web;

namespace FiveOhFirstDataCore.Api
{
    [Route("api/link")]
    [ApiController]
    public class AccountLinkController : ControllerBase
    {
        private readonly AccountLinkService _linkService;
        private readonly TrooperSignInManager _signInManager;

        public AccountLinkController(AccountLinkService linkService,
            TrooperSignInManager signInManager)
        {
            _linkService = linkService;
            _signInManager = signInManager;
        }

        [HttpGet("token/{token}")]
        public async Task<IActionResult> GetTokenEndpoint(string token)
        {
            // Use the provided token from the URL to check against the status of that tokens link
            var t = HttpUtility.HtmlDecode(token);
            switch (_linkService.GetLinkStatus(t))
            {
                case AccountLinkService.LinkStatus.Ready:
                    // If it is ready (discord and steam account sign in completed)
                    // then save the link, and tell the user.
                    var reLogin = await _linkService.FinalizeLink(token);

                    var res = await _signInManager.PasswordSignInAsync(reLogin.Item1, reLogin.Item2, reLogin.Item3, false);

                    if (res.Succeeded)
                        return Redirect("~/");
                    else return Redirect("/Identity/Account/Login");

                case AccountLinkService.LinkStatus.Waiting:
                    // If it is waiting, clear any errors, save the token as a cookie,
                    // and start the process at the login endpoint.
                    Response.Cookies.Delete("error");
                    Response.Cookies.Append("token", token);
                    return Redirect("/api/link/login");

                case AccountLinkService.LinkStatus.Invalid:
                    // If it is invalid (or default), notify the user.
                    return BadRequest("Token is invalid or not found.");

                default:
                    return BadRequest("Token is invalid, not found, or an unkown error occurred.");
            }
        }

        /// <summary>
        /// Discord OAuth2 endpoint for Account Linking
        /// </summary>
        /// <returns>The HTTP action for this endpoint.</returns>
        [HttpGet("login")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> GetLoginEndpoint()
        { // If there were any errors, return a bad request and display them.
            if (Request.Cookies.TryGetValue("error", out var error))
            {
                if (Request.Cookies.TryGetValue("token", out var token))
                { // If the token exists, abort the link.
                    await _linkService.AbortLinkAsync(token);
                }

                Response.Cookies.Delete("token");
                Response.Cookies.Delete("error");
                return BadRequest(error);
            }
            // otherwise, redirect to the FAF OAuth2 endpoint.
            return Redirect("/api/link/auth");
        }

        /// <summary>
        /// Steam OAuth2 endpoint for Account Linking
        /// </summary>
        /// <returns>The HTTP action for this endpoint.</returns>
        [HttpGet("auth")]
        [Authorize(AuthenticationSchemes = "Steam")]
        public async Task<IActionResult> GetAuthEndpoint()
        { // make sure the token exists ...
            if (Request.Cookies.TryGetValue("token", out var token))
            {
                if (Request.Cookies.TryGetValue("error", out var error))
                { // ... if there s an error, abort the link, and clear the cookies.
                    await _linkService.AbortLinkAsync(token);

                    Response.Cookies.Delete("token");
                    Response.Cookies.Delete("error");
                    return BadRequest(error);
                }
                // ... if there is no error, use the token to go back to the token endpoint for finalization.
                return Redirect($"/api/link/token/{token}");
            }

            return BadRequest("No token found.");
        }

        /// <summary>
        /// Account Link endpoint for when a user denies access for a third party login during OAuth2 sign-ins.
        /// </summary>
        /// <returns>A BadRequest HTTP result.</returns>
        [HttpGet("denied")]
        public IActionResult GetDeniedEndpoint()
        {
            return BadRequest("User denied account linking.");
        }
    }
}
