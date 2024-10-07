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
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("UserLogin", "Login");
            }

            // Fetch the user profile and account details from the business layer
            var profileResponse = await _bProfileController.RetrieveProfileByUsername(username);
            var accountResponse = await _bAccountController.GetAccountsByUsername(username);

            var userProfile = (profileResponse is OkObjectResult profileResult && profileResult.Value is Profile profile) ? profile : null;
            var userAccounts = (accountResponse is OkObjectResult accountResult && accountResult.Value is List<Account> accounts) ? accounts : new List<Account>();

            // Create a ViewModel to pass data to the view
            var dashboardViewModel = new DashboardViewModel
            {
                UserProfile = userProfile,
                UserAccounts = userAccounts
            };

            return View(dashboardViewModel);
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
