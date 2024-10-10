using Microsoft.AspNetCore.Mvc;
using System;
using DatabaseLib;

namespace BankWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly SqliteDataAccess _dataAccess;

        public LogController(IConfiguration configuration)
        {
            _dataAccess = new SqliteDataAccess(configuration);
        }

        [HttpPost("entry")]
        public IActionResult LogActivity([FromBody] ActivityLog logEntry)
        {
            try
            {
                _dataAccess.LogActivity(logEntry);
                return Ok("Activity logged successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error logging activity: {ex.Message}");
            }
        }


        [HttpGet("all")]
        public IActionResult GetAllActivityLogs()
        {
            try
            {
                List<ActivityLog> logs = _dataAccess.GetAllActivityLogs();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving activity logs: {ex.Message}");
            }
        }
    }
}
