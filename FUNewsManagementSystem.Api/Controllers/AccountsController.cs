using BO.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace FUNewsManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving accounts",
                    error = ex.Message
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById([FromRoute] int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);

                if (account == null)
                {
                    return NotFound(new
                    {
                        message = $"Account with ID {id} not found"
                    });
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving account",
                    error = ex.Message
                });
            }
        }

 
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccounts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new
                    {
                        message = "Search term cannot be empty"
                    });
                }

                var accounts = await _accountService.SearchAccountsAsync(searchTerm);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error searching accounts",
                    error = ex.Message
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdAccount = await _accountService.CreateAccountAsync(dto);

                return CreatedAtAction(
                    nameof(GetAccountById),
                    new { id = createdAccount.AccountId },
                    new
                    {
                        message = "Account created successfully",
                        data = createdAccount
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violations (email exists, invalid role...)
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating account",
                    error = ex.Message
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount([FromRoute] int id, [FromBody] UpdateAccountDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedAccount = await _accountService.UpdateAccountAsync(id, dto);

                return Ok(new
                {
                    message = "Account updated successfully",
                    data = updatedAccount
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating account",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] int id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);

                return Ok(new
                {
                    message = "Account deleted successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error deleting account",
                    error = ex.Message
                });
            }
        }
    }
}

 
