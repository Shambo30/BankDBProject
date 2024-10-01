using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;
using BankBusinessService.Models;

[Route("api/[controller]")]
[ApiController]
public class BAccountController : Controller
{
    private readonly RestClient _client = new RestClient("http://localhost:5282/api");
    private readonly ILogger<BAccountController> _logger;

    public BAccountController(ILogger<BAccountController> logger)
    {
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] Account account)
    {
        try
        {
            var request = new RestRequest("Account/create", Method.Post);
            request.AddJsonBody(account);

            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"Creating account for username: {account.holder_username} with initial balance: {account.balance}");
            return Ok(response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("retrieve/{accountId}")]
    public async Task<IActionResult> GetAccount(int accountId)
    {
        try
        {
            var request = new RestRequest($"account/retrieve/{accountId}", Method.Get);
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                // Deserialize the response content to Account object
                var account = JsonConvert.DeserializeObject<Account>(response.Content);
                _logger.LogInformation($"Retrieving account with account number: {accountId}");

                return Ok(account);
            }
            else
            {
                return StatusCode((int)response.StatusCode, response.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPost("update")]
    public async Task<IActionResult> UpdateAccount([FromBody] Account account)
    {
        try
        {
            var request = new RestRequest("Account/update", Method.Post);
            request.AddJsonBody(account);

            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"Updating account with account number: {account.account_number}, new balance: {account.balance}");

            return Ok(response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("delete/{accountId}")]
    public async Task<IActionResult> DeleteAccount(int accountId)
    {
        try
        {
            var request = new RestRequest($"Account/delete/{accountId}", Method.Post);
            var response = await _client.ExecuteAsync(request);
            _logger.LogInformation($"Deleting account with account number: {accountId}");

            return Ok(response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
