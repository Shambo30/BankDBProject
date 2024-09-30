using DatabaseLib;
using Microsoft.AspNetCore.Mvc;

namespace BankWebService.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly SqliteDataAccess _dataAccess;
        public AccountController(IConfiguration configuration)
        {
            _dataAccess = new SqliteDataAccess(configuration);
        }

        //Create a new account
        [HttpPost("create")]
        public IActionResult CreateAccount(Account account)
        {
            try
            {
                _dataAccess.AddAccount(account);
                return Ok($"Account {account.holder_username} created successfully");
            }
            catch (Exception ex) // Maybe handle custom exception
            {
                return BadRequest($"Error creating account: {ex.Message}");
            }
        }

        // Retrieve account details by account ID
        [HttpGet("retrieve/{accountId}")]
        public IActionResult GetAccount(int accountId)
        {
            try
            {
                var account = _dataAccess.GetAccountById(accountId);
                if (account == null)
                {
                    return NotFound("Account not found.");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Update account information
        [HttpPost("update")]
        public IActionResult UpdateAccount(Account account)
        {
            try
            {
                _dataAccess.UpdateAccount(account);
                return Ok($"Account {account.holder_username} updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Delete an account by account ID
        [HttpPost("delete/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            try
            {
                _dataAccess.DeleteAccount(accountId);
                return Ok($"Account {accountId} deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
