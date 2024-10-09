using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using BankBusinessService.Models;

namespace BankBusinessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BLogController : ControllerBase
    {
        private readonly RestClient _client = new RestClient("http://localhost:5282/api");
        private readonly ILogger<BLogController> _logger;

        public BLogController(ILogger<BLogController> logger)
        {
            _logger = logger;
        }

        // Internal method for logging actions in other controllers
        public async Task LogAction(string username, string action, string details)
        {
            try
            {
                var logEntry = new ActivityLog
                {
                    Username = username,
                    Action = action,
                    Details = details
                };

                var request = new RestRequest("log/entry", Method.Post);
                request.AddJsonBody(logEntry);

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    _logger.LogError($"Failed to log action: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log action.");
            }
        }

        // Handles the external api endpoint
        [HttpPost("entry")]
        public async Task<IActionResult> LogActivity([FromBody] ActivityLog logEntry)
        {
            try
            {
                // Check if the Timestamp field is null or empty, and set it if needed
                if (string.IsNullOrEmpty(logEntry.Timestamp))
                {
                    logEntry.Timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                }

                var request = new RestRequest("log/entry", Method.Post);
                request.AddJsonBody(logEntry);

                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return Ok("Activity logged successfully");
                }
                else
                {
                    _logger.LogError($"Failed to log activity: {response.ErrorMessage}");
                    return StatusCode((int)response.StatusCode, $"Error logging activity: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllActivityLogs()
        {
            try
            {
                var request = new RestRequest("Log/all", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var logs = JsonConvert.DeserializeObject<List<ActivityLog>>(response.Content);
                    _logger.LogInformation("Retrieved all activity logs successfully");

                    return Ok(logs);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity logs");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}