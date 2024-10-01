using Microsoft.AspNetCore.Mvc;
using RestSharp;
using BankBusinessService.Models;
using Newtonsoft.Json;

namespace BankBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BTransactionController : ControllerBase
    {
        private readonly RestClient _client = new RestClient("http://localhost:5282/api");
        private readonly ILogger<BTransactionController> _logger;

        public BTransactionController(ILogger<BTransactionController> logger)
        {
            _logger = logger;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
        {
            try
            {
                var restRequest = new RestRequest("Transaction/deposit", Method.Post);
                restRequest.AddJsonBody(request);

                var response = await _client.ExecuteAsync(restRequest);
                _logger.LogInformation($"Processing deposit of {request.Amount} into account {request.AccountNumber}");

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            try
            {
                var restRequest = new RestRequest("Transaction/withdraw", Method.Post);
                restRequest.AddJsonBody(request);

                var response = await _client.ExecuteAsync(restRequest);
                _logger.LogInformation($"Processing withdrawal of {request.Amount} from account {request.AccountNumber}");

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("history/{accountId}")]
        public async Task<IActionResult> GetTransactionHistory(int accountId)
        {
            try
            {
                var request = new RestRequest($"transaction/history/{accountId}", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                    _logger.LogInformation($"Retrieving transaction history for account number: {accountId}");

                    return Ok(transactions);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction history");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }

}
