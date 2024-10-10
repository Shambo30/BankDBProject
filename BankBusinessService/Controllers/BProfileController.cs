using Microsoft.AspNetCore.Mvc;
using RestSharp;
using BankBusinessService.Models;
using Newtonsoft.Json;

namespace BankBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BProfileController : ControllerBase
    {
        private readonly RestClient _client = new RestClient("http://localhost:5282/api");
        private readonly ILogger<BProfileController> _logger;
        private readonly BLogController _logController;

        public BProfileController(ILogger<BProfileController> logger, BLogController logController)
        {
            _logger = logger; // Console based logger
            _logController = logController; // Our database logger
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] Profile profile)
        {
            try
            {
                var request = new RestRequest("Profile/create", Method.Post);
                request.AddJsonBody(profile);

                var response = await _client.ExecuteAsync(request);
                _logger.LogInformation($"Creating profile for username: {profile.Username}, email: {profile.Email}, name: {profile.Name}");

                if (!response.IsSuccessful)
                {
                    return BadRequest(response.Content); // Handles if username exists already
                }

                string details = profile.Username == "admin" ? "Admin created a profile" : $"User created their own profile";
                await _logController.LogAction(profile.Username, "Profile Create", details);

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("retrieve/{username}")]
        public async Task<IActionResult> RetrieveProfileByUsername(string username)
        {
            try
            {
                var request = new RestRequest($"profile/retrieve/{username}", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var profile = JsonConvert.DeserializeObject<Profile>(response.Content);
                    _logger.LogInformation($"Retrieving profile for username: {username}");

                    string details = profile.Username == "admin" ? $"Admin retrieving profile: {username}" : $"User retrieving their own profile: {username}";
                    string user = profile.Username == "admin" ? "Admin" : $"{profile.Username}";
                    await _logController.LogAction(profile.Username, "Profile Retrieve", details);


                    return Ok(profile);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] Profile profile)
        {
            try
            {
                _logger.LogInformation($"Preparing to send update request to data layer for profile: {profile.Username}");
                _logger.LogInformation($"Data being sent - Name: {profile.Name}, Email: {profile.Email}, Address: {profile.Address}, Phone: {profile.Phone}, Picture: {profile.Picture}, Password: {profile.Password}");

                var request = new RestRequest("Profile/update", Method.Put);
                request.AddJsonBody(profile);

                var response = await _client.ExecuteAsync(request);

                _logger.LogInformation($"Data layer response status: {response.StatusCode}");
                _logger.LogInformation($"Data layer response content: {response.Content}");

                string details = profile.Username == "admin" ? $"Admin updated profile for user: {profile.Username}" : $"User updated their own profile";
                await _logController.LogAction(profile.Username, "Profile Update", details);

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("delete/{username}")]
        public async Task<IActionResult> DeleteProfile(string username)
        {
            try
            {
                var request = new RestRequest($"Profile/delete/{username}", Method.Post);
                var response = await _client.ExecuteAsync(request);
                _logger.LogInformation($"Deleting profile for username: {username}");

                string details = $"Admin deleted profile: {username}";
                await _logController.LogAction("Admin", $"Profile Delete: {username}", details);

                return Ok(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> AllProfiles()
        {
            try
            {
                var request = new RestRequest("Profile/all", Method.Get);
                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    var profiles = JsonConvert.DeserializeObject<List<Profile>>(response.Content);
                    _logger.LogInformation($"Deserialising all profiles");

                    return Ok(profiles);
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
