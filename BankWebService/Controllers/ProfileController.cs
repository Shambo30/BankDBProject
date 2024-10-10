using BankWebService.Models;
using DatabaseLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Include this for IConfiguration
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankWebService.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly SqliteDataAccess _dataAccess;
        public ProfileController(IConfiguration configuration)
        {
            _dataAccess = new SqliteDataAccess(configuration);
        }

        [HttpPost("create")]
        public IActionResult CreateNewProfile(Profile profile)
        {
            try
            {
                _dataAccess.AddProfile(profile);
                return Ok($"Profile {profile.Username} added successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // used to specifically handle the existing username scenario
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating profile: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the profile.");
            }
        }

        [HttpGet("retrieve/{username}")]
        public IActionResult RetrieveProfileByUsername(string username)
        {
            try
            {
                var profile = _dataAccess.RetrieveProfileByUsername(username);
                if (profile == null)
                {
                    return NotFound("Profile not found.");
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving profile: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the profile.");
            }
        }

        [HttpPost("delete/{username}")]
        public IActionResult DeleteProfileByUsername(string username)
        {
            try
            {
                _dataAccess.DeleteProfile(username);
                return Ok($"Profile with username {username} deleted succesfully");
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message); // handles situation where username is not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving profile: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the profile.");
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateProfile(Profile profile)
        {
            try
            {
                _dataAccess.UpdateProfile(profile);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // handles situation where profile is not found (i.e. no username match) AND if no changes are made
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the profile.");
            }
        }


        // retrieve all profiles
        [HttpGet("all")]
        public IActionResult AllProfiles()
        {
            try
            {
                var profiles = _dataAccess.AllProfiles();
                if (profiles == null || profiles.Count == 0)
                {
                    return NotFound("No profiles found");
                }
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
