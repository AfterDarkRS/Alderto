﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alderto.Data;
using Alderto.Data.Models.GuildBank;
using Microsoft.EntityFrameworkCore;

namespace Alderto.Services.GuildBankManagers
{
    public class GuildBankManager : IGuildBankManager
    {
        public ulong GuildId { get; set; }
        public ulong AdminUserId { get; set; }

        private readonly IAldertoDbContext _context;
        private readonly IGuildBankTransactionsManager _transactions;
        private readonly IGuildBankItemManager _items;

        public GuildBankManager(IAldertoDbContext context, IGuildBankTransactionsManager transactions, IGuildBankItemManager items)
        {
            _context = context;
            _transactions = transactions;
            _items = items;
        }

        public IEnumerable<GuildBankTransaction> GetAllTransactions(ulong memberId, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            var query = _context.GuildBanks.Include(b => b.Transactions) as IQueryable<GuildBank>;
            if (options != null)
                query = options(query);
            return query.Where(g => g.GuildId == GuildId).SelectMany(g => g.Transactions.Where(t => t.MemberId == memberId));
        }

        public async Task<GuildBank> GetGuildBankAsync(string name, Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            return await ((IQueryable<GuildBank>)GetAllGuildBanks(options)).SingleOrDefaultAsync(b => b.Name == name);
        }

        public IEnumerable<GuildBank> GetAllGuildBanks(Func<IQueryable<GuildBank>, IQueryable<GuildBank>> options = null)
        {
            var query = _context.GuildBanks as IQueryable<GuildBank>;
            if (options != null)
                query = options.Invoke(query);
            return query.Where(b => b.GuildId == GuildId);
        }

        public async Task ModifyCurrencyCountAsync(string bankName, ulong transactorId, double quantity, string comment = null)
        {
            var bank = await GetGuildBankAsync(bankName);
            bank.CurrencyCount += quantity;

            await _transactions.LogAsync(bank.Id, AdminUserId, transactorId, quantity, itemId: 0, comment);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyItemCountAsync(string bankName, ulong transactorId, string itemName, double quantity, string comment = null)
        {
            var bank = await GetGuildBankAsync(bankName);
            if (bank == null)
                throw new NotImplementedException();

            var item = await _items.GetItemAsync(GuildId, itemName);
            if (item == null)
                throw new NotImplementedException();

            var bankItem = await _items.GetBankItemAsync(bank.Id, item.Id) ??
                           await _items.CreateBankItemAsync(bank.Id, item.Id);

            bankItem.Quantity += quantity;

            await _transactions.LogAsync(bank.Id, AdminUserId, transactorId, quantity, bankItem.GuildBankItemId, comment);
            await _context.SaveChangesAsync();
        }

        public IGuildBankManager Configure(ulong guildId, ulong adminUserId)
        {
            GuildId = guildId;
            AdminUserId = adminUserId;

            return this;
        }

        public async Task<GuildBank> CreateGuildBankAsync(string name)
        {
            var bank = new GuildBank(GuildId, name);
            await _context.AddAsync(bank);
            await _context.SaveChangesAsync();

            return bank;
        }

        public async Task RemoveGuildBankAsync(string name)
        {
            _context.GuildBanks.Remove(await GetGuildBankAsync(name));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuildBankAsync(string name, Action<GuildBank> changes)
        {
            var gb = await GetGuildBankAsync(name);
            changes(gb);
            await _context.SaveChangesAsync();
        }
    }
}