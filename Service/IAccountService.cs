using BO.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetAllAccountsAsync();
        Task<AccountDTO?> GetAccountByIdAsync(int id);
        Task<AccountDTO> CreateAccountAsync(CreateAccountDTO dto);
        Task<AccountDTO> UpdateAccountAsync(int id, UpdateAccountDTO dto);
        Task DeleteAccountAsync(int id);
        Task<List<AccountDTO>> SearchAccountsAsync(string searchTerm);
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
    }
}
