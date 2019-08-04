﻿using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Alderto.Web.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Route("some"), Authorize]
        public IActionResult Some()
        {
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        [Authorize(AuthenticationSchemes = DiscordAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Login()
        {
            // Fetch the authentication result. It contains the access token to discord.
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Authorized using discord. Create JWT token.
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var userClaims = authResult.Principal.Claims.ToList();
            
            userClaims.Add(new Claim(ClaimTypes.Role, "User"));
            userClaims.Add(new Claim("discord_token", authResult.Properties.Items[".Token.access_token"]));

            var token = tokenHandler.CreateJwtSecurityToken(
                subject: new ClaimsIdentity(userClaims),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:SigningSecret"])),
                    SecurityAlgorithms.HmacSha256Signature),
                expires: authResult.Properties.ExpiresUtc?.DateTime
            );

            _logger.LogInformation($"User {User.Identity.Name} has logged in.");
            await HttpContext.SignOutAsync(); // Cookie is no longer needed. Sign out.

            return Content("<script>" +
                            $"window.opener.postMessage('{tokenHandler.WriteToken(token)}', '{Request.Scheme}://{Request.Host}{Request.PathBase}');" +
                             "window.close();" +
                           "</script>",
                "text/html");
        }
    }
}