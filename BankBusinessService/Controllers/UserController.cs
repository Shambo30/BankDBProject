using Microsoft.AspNetCore.Mvc;
using BankBusinessService.Models;

namespace BankBusinessService.Controllers
{
    public class UserController : Controller
    {
        private readonly BAccountController _bAccountController;
        private readonly BProfileController _bProfileController;
        private readonly BTransactionController _bTransactionController;

        public UserController(BAccountController bAccountController, BProfileController bProfileController, BTransactionController bTransactionController)
        {
            _bAccountController = bAccountController;
            _bProfileController = bProfileController;
            _bTransactionController = bTransactionController;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Retrieve username from session
            string username = HttpContext?.Session?.GetString("Username");
            
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("UserLogin", "Login");
            }

            // Check for cookies unique Session ID
            if (!HttpContext.Request.Cookies.ContainsKey("SessionID"))
            {
                return RedirectToAction("UserLogin", "Login"); // Redirect to login if not found
            }

            // Fetch the user profile from the database using the BProfileController
            var profileResponse = await _bProfileController.RetrieveProfileByUsername(username);
            if (profileResponse is OkObjectResult profileResult && profileResult.Value is Profile profile)
            {
                // Check if the profile retrieved is the admin profile
                if (profile.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("AccessDenied", "Account"); // Redirect if the admin tries to access the user dashboard
                }

                // Fetch the user's account details
                var accountResponse = await _bAccountController.GetAccountsByUsername(username);
                var userAccounts = (accountResponse is OkObjectResult accountResult && accountResult.Value is List<Account> accounts) ? accounts : new List<Account>();

                // Create a ViewModel to pass data to the view
                var dashboardViewModel = new DashboardViewModel
                {
                    UserProfile = profile,
                    UserAccounts = userAccounts
                };

                return View(dashboardViewModel);
            }

            // If no profile is found or if an error occurs, redirect to login
            return RedirectToAction("UserLogin", "Login");
        }

        //Allows for updating of user data after any transaction
        [HttpGet("GetUserAccounts")]
        public async Task<IActionResult> GetUserAccounts()
        {
            // Retrieve username from session
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            // Fetch the user accounts from the business layer
            var accountResponse = await _bAccountController.GetAccountsByUsername(username);
            if (accountResponse is OkObjectResult accountResult && accountResult.Value is List<Account> accounts)
            {
                return Ok(accounts);
            }

            return StatusCode(500, "Error retrieving user accounts.");
        }
    }
}
