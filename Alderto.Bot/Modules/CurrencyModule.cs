﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alderto.Bot.Extensions;
using Alderto.Bot.Preconditions;
using Alderto.Bot.Services;
using Alderto.Data;
using Alderto.Data.Extensions;
using Discord;
using Discord.Commands;

namespace Alderto.Bot.Modules
{
    public class CurrencyModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAldertoDbContext _context;
        private readonly IGuildPreferencesProvider _guildPreferencesProvider;
        private readonly ICurrencyProvider _currencyProvider;

        public CurrencyModule(IAldertoDbContext context, IGuildPreferencesProvider guildPreferencesProvider, ICurrencyProvider currencyProvider)
        {
            _context = context;
            _guildPreferencesProvider = guildPreferencesProvider;
            _currencyProvider = currencyProvider;
        }

        [Command("Give")]
        [Summary("Gives an amount of points to the listed users")]
        [RequireRole("Admin")]
        public async Task GiveAsync([Summary("Amount of points to give")] int qty,
            [Summary("Users to give points to")] params IGuildUser[] users)
        {
            // This is for giving, not taking
            if (qty <= 0)
            {
                await this.ReplyErrorEmbedAsync("Quantity must be greater than 0.");
                return;
            }

            if (users.Length == 0)
            {
                await this.ReplyErrorEmbedAsync("At least one user must be specified.");
                return;
            }

            var reply = await ModifyAsyncExec(qty, users);

            await ReplyAsync(embed: reply);
        }

        [Command("Take")]
        [Summary("Takes an amount of points from the listed users")]
        [RequireRole("Admin")]
        public async Task TakeAsync([Summary("Amount of points to take")] int qty,
            [Summary("Users to take points from")] params IGuildUser[] users)
        {
            // This is for taking, not giving
            if (qty <= 0)
            {
                await this.ReplyErrorEmbedAsync("No changes made: Given quantity must be > 0.");
                return;
            }

            if (users.Length == 0)
            {
                await this.ReplyErrorEmbedAsync("No changes made: At least one user must be specified.");
                return;
            }

            var reply = await ModifyAsyncExec(-qty, users);
            await ReplyAsync(embed: reply);
        }

        private async Task<Embed> ModifyAsyncExec(int qty, IEnumerable<IGuildUser> guildUsers)
        {
            var author = (IGuildUser)Context.Message.Author;
            var reply = new EmbedBuilder()
                .WithDefault(description: "**The following changes have been made:**", embedColor: EmbedColor.Success, author: author);

            var currencySymbol =
                (await _guildPreferencesProvider.GetPreferencesAsync(author.GuildId))
                .GetCurrencySymbol();

            foreach (var user in guildUsers)
            {
                var member = await _context.GetGuildMemberAsync(user.GuildId, user.Id);
                await _currencyProvider.ModifyPointsAsync(member, qty, saveChanges: false);

                // Format a nice output.
                reply.AddField($"---", $"New total for {user.Mention}: {member.CurrencyCount} {currencySymbol}");
            }

            await _context.SaveChangesAsync();

            return reply.Build();
        }

        [Command("$")]
        [Summary("Checks the amount of points a given user has.")]
        public async Task CheckAsync([Summary("Person to check. If none provided, checks personal points.")] IGuildUser user = null)
        {
            if (user == null)
                user = (IGuildUser)Context.Message.Author;

            var currencySymbol = (await _guildPreferencesProvider.GetPreferencesAsync(user.GuildId)).GetCurrencySymbol();
            var dbUser = await _context.GetGuildMemberAsync(user.GuildId, user.Id);

            await this.ReplyEmbedAsync($"{user.Mention} has {dbUser.CurrencyCount} {currencySymbol}");
        }

        [Command("Timely"), Alias("Tub", "ClaimTub")]
        public async Task Timely()
        {
            var user = (IGuildUser)Context.User;
            var dbUser = await _context.GetGuildMemberAsync(user.GuildId, user.Id);

            var preferences = await _guildPreferencesProvider.GetPreferencesAsync(user.GuildId);
            var timelyCooldown = preferences.GetTimelyCooldown();
            var currencySymbol = preferences.GetCurrencySymbol();
            var timelyAmount = preferences.GetTimelyRewardQuantity();

            var timeRemaining = dbUser.CurrencyLastClaimed.AddSeconds(timelyCooldown) - DateTimeOffset.Now;


            if (timeRemaining.Ticks > 0)
            {
                // Deny points as time delay hasn't ran out.
                await this.ReplyErrorEmbedAsync($"You will be able to claim more {currencySymbol} in **{timeRemaining}**.");
                return;
            }

            // Give out points.
            dbUser.CurrencyLastClaimed = DateTimeOffset.Now;
            dbUser.CurrencyCount += timelyAmount;
            await _context.SaveChangesAsync();

            await this.ReplySuccessEmbedAsync(($"{user.Mention} was given {timelyAmount} {currencySymbol}. New total: **{dbUser.CurrencyCount}**."));
        }
    }
}
