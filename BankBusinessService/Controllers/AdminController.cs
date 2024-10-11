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
                return RedirectToAction("AdminLogin", "Login");
            }

            // Check for cookies unique Session ID
            if (!HttpContext.Request.Cookies.ContainsKey("SessionID"))
            {
                return RedirectToAction("AdminLogin", "Login"); // Redirect to login if not found
            }

            // Fetch the user profile from the database using the BProfileController
            var profileResponse = await _bProfileController.RetrieveProfileByUsername(username);
            if (profileResponse is OkObjectResult profileResult && profileResult.Value is Profile profile)
            {
                // Check if the profile retrieved is the admin profile
                if (!profile.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("AccessDenied", "Account"); // Redirect if the user is not the admin
                }

                // Create a ViewModel to pass data to the view
                var dashboardViewModel = new DashboardViewModel
                {
                    UserProfile = profile
                };

                return View(dashboardViewModel);
            }

            // If no profile is found or if an error occurs, redirect to login
            return RedirectToAction("AdminLogin", "Login");
        }
    }
}
