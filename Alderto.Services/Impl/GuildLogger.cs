﻿using System;
using System.Threading.Tasks;
using Alderto.Data.Models.GuildBank;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace Alderto.Services.Impl
{
    public class GuildLogger : IGuildLogger
    {
        private readonly IDiscordClient _client;

        public GuildLogger(IDiscordClient client)
        {
            _client = client;
        }

        public async Task LogBankItemCreateAsync(GuildBank bank, GuildBankItem item, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var admin = await guild.GetUserAsync(modId);

            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            logMessage.WithDescription($"The following item was created in bank **{bank.Name}**:");
            logMessage
                .AddField("Name", item.Name, true)
                .AddField("Description", item.Description ?? "N/A", true)
                .AddField("Quantity", item.Quantity, true)
                .AddField("Value", item.Value, true);

            try
            {
                logMessage.WithThumbnailUrl(item.ImageUrl);
            }
            catch (ArgumentException)
            {
                // URL is not well formed. Ignore error, will not display image as it wont work in the first place.
            }

            await channel.SendMessageAsync(embed: logMessage.Build());
        }

        public async Task LogBankItemUpdateAsync(GuildBank bank, GuildBankItem oldItem, GuildBankItem newItem, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var admin = await guild.GetUserAsync(modId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            logMessage.WithDescription($"The item **{oldItem.Name}** from bank **{bank.Name}** was modified:");

            try { logMessage.WithThumbnailUrl(oldItem.ImageUrl); }
            catch (ArgumentException) { /* URL is not well formed. Ignore error, will not display image as it wont work in the first place. */ }

            var imgChanged = false;
            if (oldItem.ImageUrl != newItem.ImageUrl)
            {
                try
                {
                    imgChanged = true;
                    logMessage.WithImageUrl(newItem.ImageUrl);
                    logMessage.AddField("Image", newItem.ImageUrl, true);
                }
                catch (ArgumentException) { /* URL is not well formed. Ignore error, will not display image as it wont work in the first place. */ }
            }

            if (oldItem.Name != newItem.Name)
                logMessage.AddField("Name", $"{oldItem.Name} -> {newItem.Name}", true);
            if (oldItem.Description != newItem.Description)
                logMessage.AddField("Description", $"{oldItem.Description ?? "N/A"} -> {newItem.Description ?? "N/A"}", true);
            if (Math.Abs(oldItem.Quantity - newItem.Quantity) > 0.000001)
                logMessage.AddField("Quantity", $"{oldItem.Quantity} -> {newItem.Quantity}", true);
            if (Math.Abs(oldItem.Value - newItem.Value) > 0.000001)
                logMessage.AddField("Value", $"{oldItem.Value} -> {newItem.Value}", true);

            if (logMessage.Fields.Count > 0 || imgChanged)
                await channel.SendMessageAsync(embed: logMessage.Build()).ConfigureAwait(false);
        }

        public async Task LogBankItemQuantityUpdateAsync(GuildBank bank, GuildBankItem item, double amount, ulong modId, ulong? transactorId = null)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var admin = await guild.GetUserAsync(modId);
            var transactor = await guild.GetUserAsync(transactorId ?? modId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(transactor)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            try { logMessage.WithThumbnailUrl(item.ImageUrl); }
            catch (ArgumentException) { /* URL is not well formed. Ignore error, will not display image as it wont work in the first place. */ }

            var action = amount > 0 ? "Picked up" : "Deposited";

            logMessage.WithDescription($"{action} **{amount}** **{item.Name}** from **{bank.Name}**. New Total: **{item.Quantity}**.");

            await channel.SendMessageAsync(embed: logMessage.Build()).ConfigureAwait(false);
        }

        public async Task LogBankItemDeleteAsync(GuildBank bank, GuildBankItem item, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var admin = await guild.GetUserAsync(modId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            logMessage.WithDescription($"The following item was deleted from bank **{bank.Name}**:");
            logMessage
                .AddField("Name", item.Name, true)
                .AddField("Description", item.Description ?? "N/A", true)
                .AddField("Quantity", item.Quantity, true)
                .AddField("Value", item.Value, true);

            try { logMessage.WithThumbnailUrl(item.ImageUrl); }
            catch (ArgumentException) { /* URL is not well formed. Ignore error, will not display image as it wont work in the first place. */ }

            await channel.SendMessageAsync(embed: logMessage.Build());
        }

        public async Task LogBankCreateAsync(GuildBank bank, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var admin = await guild.GetUserAsync(modId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            logMessage.WithDescription("The following bank was created:");
            logMessage
                .AddField("Name", $"{bank.Name}", true)
                .AddField("Log Channel", $"<#{bank.LogChannelId}>", true);

            await channel.SendMessageAsync(embed: logMessage.Build());
        }

        public async Task LogBankUpdateAsync(GuildBank oldBank, GuildBank newBank, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (newBank.LogChannelId == null && oldBank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(oldBank.GuildId);
            var admin = await guild.GetUserAsync(modId);

            // Special case: Log channel change in old channel and log other changes in the updated channel.
            // Ensure that old bank has a log channel.
            // Ensure that the log channel ids differ.
            if (oldBank.LogChannelId != null && oldBank.LogChannelId != newBank.LogChannelId)
            {
                var c = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)oldBank.LogChannelId);
                var comment = newBank.LogChannelId == null
                    ? $"Log channel for bank **{oldBank.Name}** was removed."
                    : $"Log channel for bank **{oldBank.Name}** was changed to <#{newBank.LogChannelId}>.";
                try
                {
                    await c.SendMessageAsync(embed: new EmbedBuilder()
                        .WithAuthor(admin)
                        .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}")
                        .WithDescription(comment).Build());
                }
                catch (HttpException) { /* Ignore error. Bot most likely does not have access to previous channel anymore. No point disallowing log channel change. */ }
            }

            // Ensure that the updated bank has a log channel.
            if (newBank.LogChannelId == null)
                return;

            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)newBank.LogChannelId);

            // There is only one value to edit.
            if (oldBank.Name != newBank.Name)
            {
                var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

                logMessage.WithDescription($"The bank **{oldBank.Name}** was modified:");
                logMessage.AddField("Name", $"{oldBank.Name} -> {newBank.Name}", true);
                await channel.SendMessageAsync(embed: logMessage.Build());
            }
        }

        public async Task LogBankDeleteAsync(GuildBank bank, ulong modId)
        {
            // Do not log if there is nowhere to log.
            if (bank.LogChannelId == null)
                return;

            var guild = await _client.GetGuildAsync(bank.GuildId);
            var channel = (ISocketMessageChannel)await guild.GetChannelAsync((ulong)bank.LogChannelId);

            var admin = await guild.GetUserAsync(modId);

            var logMessage = new EmbedBuilder()
                .WithAuthor(admin)
                .WithFooter($"Req. by {admin.Username}#{admin.Discriminator}");

            logMessage.WithDescription("The following bank was deleted:");

            logMessage
                .AddField("Name", $"{bank.Name}", true)
                .AddField("Log Channel", $"<#{bank.LogChannelId}>", true);

            await channel.SendMessageAsync(embed: logMessage.Build());

            // Offload logging of item deletions to another thread.
            _ = Task.Factory.StartNew(async () =>
            {
                if (bank.Contents != null)
                    foreach (var item in bank.Contents)
                        await LogBankItemDeleteAsync(bank, item, modId);
            });
        }
    }
}