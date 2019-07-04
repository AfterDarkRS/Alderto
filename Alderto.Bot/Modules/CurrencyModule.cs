﻿using System;
using System.Threading.Tasks;
using Alderto.Bot.Preconditions;
using Alderto.Data;
using Alderto.Data.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace Alderto.Bot.Modules
{
    public class CurrencyModule : ModuleBase<SocketCommandContext>
    {
        private readonly SqliteDbContext _context;

        public CurrencyModule(SqliteDbContext context)
        {
            _context = context;
        }

        [Command("Give")]
        [Summary("Gives an amount of points to the listed users")]
        [RequireRole("Admin")]
        public async Task GiveAsync([Summary("Amount of points to give")] int qty,
            [Summary("People to give points to")] params SocketUser[] users)
        {
            var totalChanges = 0;
            foreach (var user in users)
            {
                var dbUser =
                    await _context.Members.SingleOrDefaultAsync(m => m.MemberId == user.Id && m.GuildId == Context.Guild.Id);
                if (dbUser == null)
                {
                    dbUser = new Member
                    {
                        GuildId = Context.Guild.Id,
                        MemberId = user.Id
                    };
                    await _context.Members.AddAsync(dbUser);
                    totalChanges += await _context.SaveChangesAsync();
                }

                dbUser.CurrencyCount += qty;
            }

            totalChanges += await _context.SaveChangesAsync();
            await ReplyAsync($"{totalChanges} user(s) have been granted {qty} points.");
        }

        [Command("Points")]
        [Summary("Ghecks the amount of points a given user has.")]
        public async Task CheckAsync([Summary("Person to check. If none provided, checks personal points.")] IGuildUser user = null)
        {
            if (user == null)
                user = (IGuildUser) Context.Message.Author;

            var dbUser =
                await _context.Members.SingleOrDefaultAsync(m => m.MemberId == user.Id && m.GuildId == Context.Guild.Id) ??
                new Member();

            await ReplyAsync($"```{user.Nickname} [{user.Username}#{user.Discriminator}] has {dbUser.CurrencyCount} point(s).```");
        }
    }
}
