using Microsoft.AspNetCore.Mvc;
using BankBusinessService.Models;

namespace BankBusinessService.Controllers
{
    public class LoginController : Controller
    {
        private readonly BProfileController _bProfileController;
        private readonly BAccountController _bAccountController;

        // Dependency Injection to get BProfileController
        public LoginController(BProfileController bProfileController, BAccountController bAccountController)
        {
            _bProfileController = bProfileController;
            _bAccountController = bAccountController;
        }

        public IActionResult UserLogin()
        {
            return View();
        }
        public IActionResult AdminLogin()
        {
            return View();
        }

        // called when admin login is authenticated
        [HttpPost]
        public IActionResult AdminAuth(string username, string password)
        {
            if (username == "admin" && password == "admin") // basic, one account check.
            {
                // dynamic cookie generator
                // GUID = Globally Unique ID.
                var sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("SessionID", sessionId, new CookieOptions
                {
                    HttpOnly = true, // Helps mitigate XSS attacks
                    Secure = true, // Ensures the cookie is only sent over HTTPS
                    SameSite = SameSiteMode.Strict // Protects against CSRF attacks
                });

                return RedirectToAction("Dashboard", "Admin");
            }

            // If invalid:
            ModelState.AddModelError("", "Invalid username or password.");
            return View("AdminLogin");
        }

        // called when user login is authenticated
        [HttpPost]
        public async Task<IActionResult> UserAuth(string username, string password)
        {
            // Profile profile = JsonConvert<Profile>(ProfileController.GetProfile(id)) - might not be the exact code... 
            Console.WriteLine($"[LoginController] Attempting login for username: {username} with password: {password}");
            // Retrieve the user profile using BProfileController
            var response = await _bProfileController.RetrieveProfileByUsername(username); // Call to BProfileController to fetch user profile

            if (response is OkObjectResult okResult && okResult.Value is Profile profile)
            {
                Console.WriteLine($"Profile found for username: {profile.Username}");
                //if (id == "admin" && password == "admin") // basic, one account check.
                //if (profile != null && id == Profile.id && password == Profile.password)

                // Validate the password against the retrieved profile
                if (profile.Password == password)
                {
                    Console.WriteLine("Password is correct. Setting session and redirecting to dashboard.");
                    // Store the username in session for later use
                    HttpContext.Session.SetString("Username", profile.Username);
                    Console.WriteLine($"Username {profile.Username} stored in session.");

                    var sessionId = Guid.NewGuid().ToString();
                    Response.Cookies.Append("SessionID", sessionId, new CookieOptions
                    {
                        HttpOnly = true, // Helps mitigate XSS attacks
                        Secure = true, // Ensures the cookie is only sent over HTTPS
                        SameSite = SameSiteMode.Strict // Protects against CSRF attacks
                    });

                    // Redirect to UserController's Dashboard with all necessary data in session
                    return RedirectToAction("Dashboard", "User");
                }
                else
                {
                    Console.WriteLine("Password does not match.");
                }
            }
            else
            {
                Console.WriteLine("Profile not found for the provided username.");
            }

            // If invalid:
            ModelState.AddModelError("", "Invalid account ID or password.");
            Console.WriteLine("User or password is incorrect.");
            return View("UserLogin");
        }
    }
}
