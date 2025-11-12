using BO;
using BO.Dto;
using Microsoft.Extensions.Configuration;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AccountService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<List<AccountDTO>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAccountsAsync();
            return accounts.Select(MapToDTO).ToList();
        }
        public async Task<AccountDTO?> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            return account != null ? MapToDTO(account) : null;
        }

        public async Task<AccountDTO> CreateAccountAsync(CreateAccountDTO dto)
        {
            // Validate: Email 
            bool isEmailUnique = await _accountRepository.IsEmailUniqueAsync(dto.AccountEmail);
            if (!isEmailUnique)
            {
                throw new InvalidOperationException($"Email '{dto.AccountEmail}' already exists");
            }

            if (dto.AccountRole < 1 || dto.AccountRole > 2)
            {
                throw new InvalidOperationException("Role must be 1 (Staff) or 2 (Lecturer)");
            }

            var account = new SystemAccount
            {
                AccountName = dto.AccountName.Trim(),
                AccountEmail = dto.AccountEmail.Trim().ToLower(),
                AccountRole = dto.AccountRole,
                AccountPassword = dto.AccountPassword
            };

            var createdAccount = await _accountRepository.CreateAccountAsync(account);

            return MapToDTO(createdAccount);
        }
        public async Task<AccountDTO> UpdateAccountAsync(int id, UpdateAccountDTO dto)
        {
            var existingAccount = await _accountRepository.GetAccountByIdAsync(id);
            if (existingAccount == null)
            {
                throw new InvalidOperationException($"Account with ID {id} not found");
            }

            bool isEmailUnique = await _accountRepository.IsEmailUniqueAsync(dto.AccountEmail, id);
            if (!isEmailUnique)
            {
                throw new InvalidOperationException($"Email '{dto.AccountEmail}' already exists");
            }

            if (dto.AccountRole < 1 || dto.AccountRole > 2)
            {
                throw new InvalidOperationException("Role must be 1 (Staff) or 2 (Lecturer)");
            }

            existingAccount.AccountName = dto.AccountName.Trim();
            existingAccount.AccountEmail = dto.AccountEmail.Trim().ToLower();
            existingAccount.AccountRole = dto.AccountRole;

            if (!string.IsNullOrWhiteSpace(dto.AccountPassword))
            {
                existingAccount.AccountPassword = dto.AccountPassword;
            }

            await _accountRepository.UpdateAccountAsync(existingAccount);

            return MapToDTO(existingAccount);
        }
        public async Task DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);
            if (account == null)
            {
                throw new InvalidOperationException($"Account with ID {id} not found");
            }

            bool canDelete = await _accountRepository.CanDeleteAccountAsync(id);
            if (!canDelete)
            {
                throw new InvalidOperationException(
                    $"Cannot delete account '{account.AccountName}' because it has created news articles. " +
                    "Please delete or reassign the news articles first.");
            }

            await _accountRepository.DeleteAccountAsync(id);
        }
        public async Task<List<AccountDTO>> SearchAccountsAsync(string searchTerm)
        {
            var accounts = await _accountRepository.SearchAccountsAsync(searchTerm);
            return accounts.Select(MapToDTO).ToList();
        }
        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(dto.Email);

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            var adminName = _configuration["AdminAccount:Name"];

            if (dto.Email == adminEmail)
            {
                if (dto.Password != adminPassword)
                {
                    throw new InvalidOperationException("Invalid email or password");
                }

                var adminAccount = new SystemAccount
                {
                    AccountId = 0, // Admin ID = 0
                    AccountName = adminName ?? "Administrator",
                    AccountEmail = adminEmail ?? "",
                    AccountRole = 0 // Admin role = 0
                };

                // Generate JWT token
                var adminToken = _jwtService.GenerateToken(adminAccount);

                return new LoginResponseDTO
                {
                    Token = adminToken,
                    Account = MapToDTO(adminAccount)
                };
            }

            if (account == null)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            // Verify password
          
            if (dto.Password != account.AccountPassword)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(account);

            return new LoginResponseDTO
            {
                Token = token,
                Account = MapToDTO(account)
            };
        }

        /// <summary>
        /// Helper method: Map Entity to DTO
        /// </summary>
        private AccountDTO MapToDTO(SystemAccount account)
        {
            return new AccountDTO
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };
        }
    }
}
