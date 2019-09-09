﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alderto.Data;
using Alderto.Data.Models;
using Alderto.Data.Models.GuildBank;
using Microsoft.EntityFrameworkCore;

namespace Alderto.Services.GuildBankManagers
{
    public class GuildBankManager : IGuildBankManager
    {

        private readonly IAldertoDbContext _context;
        private readonly IGuildBankTransactionsManager _transactions;
        private readonly IGuildBankItemManager _items;

        public GuildBankManager(IAldertoDbContext context, IGuildBankTransactionsManager transactions, IGuildBankItemManager items)
        {
            _context = context;
            _transactions = transactions;
            _items = items;
        }

        private IQueryable<GuildBank> FetchGuildBanks(ulong guildId, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            var query = _context.GuildBanks as IQueryable<GuildBank>;
            if (options != null)
                query = options.Invoke(query);
            return query.Where(b => b.GuildId == guildId);
        }

        public Task<GuildBank> GetGuildBankAsync(ulong guildId, string name, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            return FetchGuildBanks(guildId, options).SingleOrDefaultAsync(b => b.Name == name);
        }

        public Task<GuildBank> GetGuildBankAsync(ulong guildId, int id, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            return FetchGuildBanks(guildId, options).SingleOrDefaultAsync(b => b.Id == id);
        }

        public Task<List<GuildBank>> GetAllGuildBanksAsync(ulong guildId, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            return FetchGuildBanks(guildId, options).ToListAsync();
        }
        
        public async Task ModifyItemCountAsync(ulong guildId, string bankName, ulong adminId, ulong transactorId, string itemName, double quantity, string comment = null)
        {
            var bank = await GetGuildBankAsync(guildId, bankName);
            if (bank == null)
                throw new NotImplementedException();

            var item = await _items.GetBankItemAsync(bank.Id, itemName);
            if (item == null)
                throw new NotImplementedException();

            item.Quantity += quantity;

            await _context.SaveChangesAsync();
        }

        public async Task<GuildBank> CreateGuildBankAsync(ulong guildId, string name, ulong? logChannelId = null)
        {
            // If guild id is unspecified do nothing.
            if (guildId == 0)
                return null;

            // Ensure foreign key constraint is not violated.
            var guild = await _context.Guilds.FindAsync(guildId);
            if (guild == null)
            {
                guild = new Guild(guildId);
                await _context.Guilds.AddAsync(guild);
            }

            // Add the bank
            var bank = new GuildBank(guildId, name) { LogChannelId = logChannelId };
            await _context.GuildBanks.AddAsync(bank);

            // Save changes
            await _context.SaveChangesAsync();

            // Return the added bank
            return bank;
        }

        public async Task RemoveGuildBankAsync(ulong guildId, string name)
        {
            _context.GuildBanks.Remove(await GetGuildBankAsync(guildId, name));
            await _context.SaveChangesAsync();
        }

        public async Task RemoveGuildBankAsync(ulong guildId, int id)
        {
            _context.GuildBanks.Remove(await GetGuildBankAsync(guildId, id));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuildBankAsync(ulong guildId, string name, Action<GuildBank> changes)
        {
            var gb = await GetGuildBankAsync(guildId, name);
            changes(gb);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuildBankAsync(ulong guildId, int id, Action<GuildBank> changes)
        {
            var gb = await GetGuildBankAsync(guildId, id);
            changes(gb);
            await _context.SaveChangesAsync();
        }
    }
}