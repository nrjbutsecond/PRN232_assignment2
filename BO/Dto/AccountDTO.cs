using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO.Dto
{
    //account response
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail {  get; set; } = string.Empty;
        public int AccountRole { get; set; }
    }

    public class CreateAccountDTO
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole { get; set; }
        public string AccountPassword {  get; set; } = string.Empty;
    }

    public class UpdateAccountDTO
    {
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole { get; set; }
        public string AccountPassword { get; set; }
    }

    public class LoginDTO
    {
        public string Email {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public AccountDTO Account { get; set; } = null!;
    }
}
