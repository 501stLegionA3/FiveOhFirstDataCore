using FiveOhFirstDataCore.Core.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FiveOhFirstDataCore.Api
{
    [Route("api/link")]
    [ApiController]
    public class AccountLinkController : ControllerBase
    {
		private readonly AccountLinkService _linkService;

		public AccountLinkController(AccountLinkService linkService)
		{
			_linkService = linkService;
		}

		[HttpGet("token/{token}")]
        public async Task<IActionResult> GetTokenEndpoint(string token)
        {
			// Use the proivded token from the URL to check against the status of that tokens link
			var t = HttpUtility.HtmlDecode(token);
			switch (_linkService.GetLinkStatus(t))
			{
				case AccountLinkService.LinkStatus.Ready:
					// If it is ready (discord and steam account sign in completed)
					// then save the link, and tell the user.
					_ = Task.Run(async () => await _linkService.FinalizeLink(token));

					return Redirect("/Identity/Account/Login");

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
					return BadRequest("Token is invalid, not found, or an unkown error occoured.");
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
				{ // If the token exsists, abort the link.
					await _linkService.AbortLinkAsync(token);
				}

				Response.Cookies.Delete("token");
				Response.Cookies.Delete("error");
				return BadRequest(error);
			}
			// otherwise, redirect to the FAF OAtuh2 endpoint.
			return Redirect("/api/link/auth");
		}

		/// <summary>
		/// Steam OAuth2 endpoint for Account Linking
		/// </summary>
		/// <returns>The HTTP action for this endpoint.</returns>
		[HttpGet("auth")]
		[Authorize(AuthenticationSchemes = "Steam")]
		public async Task<IActionResult> GetAuthEndpoint()
		{ // make sure the token exsists ...
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
