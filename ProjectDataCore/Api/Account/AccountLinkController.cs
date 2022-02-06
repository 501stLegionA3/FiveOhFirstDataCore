using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ProjectDataCore.Data.Services.Account;
using System.Web;

using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ProjectDataCore.Api.Account;

[Route("api/link")]
[ApiController]
public class AccountLinkController : ControllerBase
{
    private readonly IAccountLinkService _linkService;
    private readonly DataCoreSignInManager _signInManager;

    public AccountLinkController(IAccountLinkService linkService,
        DataCoreSignInManager signInManager)
    {
        _linkService = linkService;
        _signInManager = signInManager;
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> GetTokenEndpoint(string token)
    {
        // Use the provided token from the URL to check against the status of that tokens link
        var t = HttpUtility.HtmlDecode(token);
        var status = _linkService.GetLinkStatus(t);
        switch (status.Item1)
        {
            case AccountLinkService.LinkStatus.Ready:
                // If it is ready (links required by the system completed)
                // then save the link, and tell the user.
                var reLogin = await _linkService.FinalizeLink(token);

                var res = await _signInManager.PasswordSignInAsync(reLogin.Item1, reLogin.Item2, reLogin.Item3, false);

                if (res.Succeeded)
                    return Redirect("~/");
                else return Redirect("/Login");

            case AccountLinkService.LinkStatus.Waiting:
                // If it is waiting, clear any errors, save the token as a cookie,
                // and start the process at the login endpoint
                Response.Cookies.Delete("redir");
                Response.Cookies.Delete("error");
                Response.Cookies.Append("token", token);
                return Redirect(status.Item2!);

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
    [HttpGet("discord")]
    [Authorize(AuthenticationSchemes = "Discord")]
    public async Task<IActionResult> GetLoginEndpoint()
    { // If there were any errors, return a bad request and display them.
        if (Request.Cookies.TryGetValue("error", out var error))
        {
            if (Request.Cookies.TryGetValue("token", out var token))
            { // If the token exists, abort the link.
                await _linkService.AbortLinkAsync(token!);
            }

            Response.Cookies.Delete("token");
            Response.Cookies.Delete("error");
            Response.Cookies.Delete("redir");
            return BadRequest(error);
        }

        if(Request.Cookies.TryGetValue("redir", out var redir))
        {
            // otherwise, redirect to the next auth.
            return Redirect(redir!);
        }
        else
        {
            if (Request.Cookies.TryGetValue("token", out var token))
            {
                return Redirect($"/api/link/token/{token}");
            }

            return BadRequest("Failed to get the next linking endpoint. The linking token may have expired.");
        }
    }

    /// <summary>
    /// Steam OAuth2 endpoint for Account Linking
    /// </summary>
    /// <returns>The HTTP action for this endpoint.</returns>
    [HttpGet("steam")]
    [Authorize(AuthenticationSchemes = "Steam")]
    public async Task<IActionResult> GetAuthEndpoint()
    { // make sure the token exists ...
        if (Request.Cookies.TryGetValue("token", out var token))
        {
            if (Request.Cookies.TryGetValue("error", out var error))
            { // ... if there s an error, abort the link, and clear the cookies.
                await _linkService.AbortLinkAsync(token!);

                Response.Cookies.Delete("token");
                Response.Cookies.Delete("error");
                Response.Cookies.Delete("redir");
                return BadRequest(error);
            }

            if (Request.Cookies.TryGetValue("redir", out var redir))
            {
                // otherwise, redirect to the next auth.
                return Redirect(redir!);
            }
            else
            {
                return Redirect($"/api/link/token/{token}");
            }
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
