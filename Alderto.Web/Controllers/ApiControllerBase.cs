﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Alderto.Web.Controllers
{
    [ApiController, Authorize]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Forbid(ErrorMessage message)
        {
            return StatusCode(StatusCodes.Status403Forbidden, message);
        }

        protected IActionResult BadRequest(ErrorMessage message)
        {
            return StatusCode(StatusCodes.Status400BadRequest, message);
        }

        protected IActionResult NotFound(ErrorMessage message)
        {
            return StatusCode(StatusCodes.Status404NotFound, message);
        }

        protected IActionResult Content(object data)
        {
            return StatusCode(StatusCodes.Status200OK, data);
        }

        protected class ErrorMessage
        {
            public int Code { get; set; }
            public string Message { get; set; }

            public ErrorMessage(int code, string message)
            {
                Code = code;
                Message = message;
            }
        }

        protected static class ErrorMessages
        {
            // === Forbid ===
            
            /// <summary>
            /// Forbid reason. Used when unable to confirm admin status.
            /// </summary>
            public static ErrorMessage UserNotDiscordAdmin { get; } =
                new ErrorMessage(1100, "Could not confirm if user is an admin of the specified guild.");

            /// <summary>
            /// Forbid reason. Used when unable to confirm bank moderator status.
            /// </summary>
            public static ErrorMessage UserNotBankModerator { get; } =
                new ErrorMessage(1200, "Could not confirm if user has access to add or remove items form the specified bank.");

            // === NotFound ===

            /// <summary>
            /// NotFound reason. Used when bot was unable to find the guild.
            /// </summary>
            public static ErrorMessage GuildNotFound { get; } =
                new ErrorMessage(2100, "The specified guild was not found.");

            /// <summary>
            /// NotFound reason. Used when bot was unable to find the user.
            /// </summary>
            public static ErrorMessage UserNotFound { get; } =
                new ErrorMessage(2101, "The specified user was not found.");

            /// <summary>
            /// NotFound reason. Used when bot was unable to find the bank.
            /// </summary>
            public static ErrorMessage BankNotFound { get; } =
                new ErrorMessage(2200, "The specified bank was not found.");

            // === BadRequest ===

            /// <summary>
            /// BadRequest reason. Used when received over 100 guilds in a request.
            /// </summary>
            public static ErrorMessage GuildCountOver100 { get; } =
                new ErrorMessage(3100, "Guild count cannot exceed 100.");

            /// <summary>
            /// BadRequest reason. Used when bank with the given name already exists.
            /// </summary>
            public static ErrorMessage BankNameAlreadyExists { get; } =
                new ErrorMessage(3200, "A bank with the given name already exists.");
        }
    }
}