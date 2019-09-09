﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alderto.Data.Models.GuildBank;

namespace Alderto.Services.GuildBankManagers
{
    public interface IGuildBankManager
    {
        /// <summary>
        /// Gets the guild bank with the provided name.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="name">Name of the bank.</param>
        /// <param name="options">Additional includes.</param>
        /// <returns>Guild bank with the given name.</returns>
        Task<GuildBank> GetGuildBankAsync(ulong guildId, string name,
            Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null);

        /// <summary>
        /// Gets the guild bank with the provided name.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="id">Id of the bank.</param>
        /// <param name="options">Additional includes.</param>
        /// <returns>Guild bank with the given id.</returns>
        Task<GuildBank> GetGuildBankAsync(ulong guildId, int id, 
            Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null);

        /// <summary>
        /// Gets all banks of the configured guild.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="options">Additional includes.</param>
        /// <returns>A collection of guild banks belonging to the given guild.</returns>
        Task<List<GuildBank>> GetGuildBanksAsync(ulong guildId, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null);

        /// <summary>
        /// Adds a Guild Bank to the database.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="adminId">Id of administrator user.</param>
        /// <param name="name">Name of bank to add to be added to the database.</param>
        /// <param name="logChannelId">Id of channel to log info to.</param>
        Task<GuildBank> CreateGuildBankAsync(ulong guildId, ulong adminId, string name, ulong? logChannelId = null);

        /// <summary>
        /// Removes a guild bank of a given name.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="name">Name of guild bank to remove.</param>
        Task RemoveGuildBankAsync(ulong guildId, string name);

        /// <summary>
        /// Removes a guild bank of a given name.
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="id">Id of guild bank to remove.</param>
        Task RemoveGuildBankAsync(ulong guildId, int id);

        /// <summary>
        /// Updates the guild bank as described in <see cref="changes"/>
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="name">Name of bank to update.</param>
        /// <param name="changes">Changes to apply.</param>
        Task UpdateGuildBankAsync(ulong guildId, string name, Action<GuildBank> changes);

        /// <summary>
        /// Updates the guild bank as described in <see cref="changes"/>
        /// </summary>
        /// <param name="guildId">Guild Id.</param>
        /// <param name="id">Id of bank to update.</param>
        /// <param name="changes">Changes to apply.</param>
        Task UpdateGuildBankAsync(ulong guildId, int id, Action<GuildBank> changes);
    }
}