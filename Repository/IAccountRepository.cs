using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<List<SystemAccount>> GetAllAccountsAsync();
        Task<SystemAccount?> GetAccountByIdAsync(int id);
        Task<SystemAccount?> GetAccountByEmailAsync(string email);
        Task<SystemAccount> CreateAccountAsync(SystemAccount account);
        Task UpdateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(int id);
        Task<bool> CanDeleteAccountAsync(int accountId);
        Task<List<SystemAccount>> SearchAccountsAsync(string searchTerm);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeAccountId = null);
    }
}
