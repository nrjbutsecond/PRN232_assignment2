using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAO
{
    public class AccountDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public AccountDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemAccount>> GetAllAsync()
        {
            return await _context.SystemAccounts
                .OrderBy(a => a.AccountName)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<SystemAccount?> GetByIdAsync(int id)
        {
            return await _context.SystemAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<SystemAccount> AddAsync(SystemAccount account)
        {
            if (account == null)
            {
                throw new ArgumentException(nameof(account));

            }
            _context.SystemAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            if (account == null)
                throw new ArgumentException(nameof(account));
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _context.SystemAccounts.FirstOrDefaultAsync(a => a.AccountId == id);
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasNewsArticlesAsync(int accountId)
        {
            return await _context.NewsArticles.AnyAsync(n => n.CreatedById == accountId);
        }

        public async Task<List<SystemAccount>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower().Trim();

            return await _context.SystemAccounts
                .Where(a => a.AccountName.ToLower().Contains(searchTerm) || a.AccountEmail.ToLower().Contains(searchTerm))
                .OrderBy(a => a.AccountName)
                .AsNoTracking().ToListAsync();


        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeAccountId)
        {
            var query = _context.SystemAccounts.Where(a => a.AccountEmail == email);

            if (excludeAccountId.HasValue)
            {
                query = query.Where(a => a.AccountId != excludeAccountId.Value);
            }
            return await query.AnyAsync();
        }
        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _context.SystemAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
        }
    }
}
