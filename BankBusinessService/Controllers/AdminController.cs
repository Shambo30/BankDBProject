using BankBusinessService.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankBusinessService.Controllers
{
    public class AdminController : Controller
    {
        private readonly BProfileController _bProfileController;

        public AdminController(BProfileController bProfileController)
        {
            _bProfileController = bProfileController;
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

            var userProfile = (profileResponse is OkObjectResult profileResult && profileResult.Value is Profile profile) ? profile : null;

            // Create a ViewModel to pass data to the view
            var dashboardViewModel = new DashboardViewModel
            {
                UserProfile = userProfile
            };

            return View(dashboardViewModel);
        }
    }
}
