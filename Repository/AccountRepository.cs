using BO;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;
        public AccountRepository(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        public async Task<List<SystemAccount>> GetAllAccountsAsync()
        {
            return await _accountDAO.GetAllAsync();
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(int id)
        {
            return await _accountDAO.GetByIdAsync(id);
        }

        public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
        {
            return await _accountDAO.GetByEmailAsync(email);
        }

        public async Task<SystemAccount> CreateAccountAsync(SystemAccount account)
        {
            return await _accountDAO.AddAsync(account);
        }

        public async Task UpdateAccountAsync(SystemAccount account)
        {
            await _accountDAO.UpdateAsync(account);
        }

        public async Task DeleteAccountAsync(int id)
        {
            await _accountDAO.DeleteAsync(id);
        }

  
        public async Task<bool> CanDeleteAccountAsync(int accountId)
        {
            bool hasNews = await _accountDAO.HasNewsArticlesAsync(accountId);
            return !hasNews; 
        }

        public async Task<List<SystemAccount>> SearchAccountsAsync(string searchTerm)
        {
            return await _accountDAO.SearchAsync(searchTerm);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeAccountId = null)
        {
            bool exists = await _accountDAO.EmailExistsAsync(email, excludeAccountId);
            return !exists; 
        }
    }
}