﻿using System.Linq;
using System.Threading.Tasks;
using Alderto.Web.Extensions;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace Alderto.Web.Controllers
{
    [Route("api/guilds/{guildId}")]
    public class GuildController : ApiControllerBase
    {
        private readonly DiscordSocketClient _client;

        public GuildController(DiscordSocketClient client)
        {
            _client = client;
        }

        [HttpGet("channels")]
        public async Task<IActionResult> Channels(ulong guildId)
        {
            if (!await User.IsDiscordAdminAsync(guildId))
                return Forbid(ErrorMessages.UserNotDiscordAdmin);

            return Content(_client.GetGuild(guildId).TextChannels.Select(c => new { c.Id, c.Name }));
        }
    }
}