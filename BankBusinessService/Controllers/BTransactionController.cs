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
                _logger.LogInformation($"Processing deposit of {request.Amount} into account {request.RecipientAccount}");

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
                _logger.LogInformation($"Processing withdrawal of {request.Amount} from account {request.SenderAccount}");

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

                if (response.IsSuccessful)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        var transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                        _logger.LogInformation($"Retrieving transaction history for account number: {accountId}");
                        return Ok(transactions);
                    }
                    else
                    {
                        _logger.LogInformation($"No transaction history found for account number: {accountId}");
                        return NoContent(); // Return 204 No Content if the account exists but has no transactions
                    }
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



        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransactionRequest transactionRequest)
        {
            try
            {
                // Check if the sender's account has sufficient funds
                var senderAccountResponse = await _client.ExecuteAsync(new RestRequest($"account/retrieve/{transactionRequest.SenderAccount}", Method.Get));
                var senderAccount = JsonConvert.DeserializeObject<Account>(senderAccountResponse.Content);

                if (senderAccount.balance < transactionRequest.Amount)
                {
                    _logger.LogInformation($"Transfer failed: Insufficient funds in sender account {transactionRequest.SenderAccount}");
                    return BadRequest("Insufficient funds in the sender's account.");
                }

                // Check if the recipient's account exists
                var recipientAccountResponse = await _client.ExecuteAsync(new RestRequest($"account/retrieve/{transactionRequest.RecipientAccount}", Method.Get));
                var recipientAccount = JsonConvert.DeserializeObject<Account>(recipientAccountResponse.Content);

                if (recipientAccount == null)
                {
                    _logger.LogInformation($"Transfer failed: Invalid recipient account {transactionRequest.RecipientAccount}");
                    return BadRequest("Invalid recipient account.");
                }

                // Withdraw the amount from the sender's account
                var withdrawResponse = await Withdraw(new TransactionRequest
                {
                    SenderAccount = transactionRequest.SenderAccount,
                    Amount = transactionRequest.Amount
                });

                // Deposit the amount into the recipient's account
                var depositResponse = await Deposit(new TransactionRequest
                {
                    RecipientAccount = transactionRequest.RecipientAccount,
                    Amount = transactionRequest.Amount
                });

                _logger.LogInformation($"Successfully transferred {transactionRequest.Amount} from account {transactionRequest.SenderAccount} to account {transactionRequest.RecipientAccount}");
                return Ok("Transfer successful!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing money transfer");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> AllTransactions() 
        {
            try
            {
                var request = new RestRequest("transaction/all", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                    _logger.LogInformation($"Deserialising all transactions");

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
