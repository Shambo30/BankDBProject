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
        public async Task<IActionResult> AdminAuth(string username, string password)
        {
            Console.WriteLine($"[LoginController] Attempting login for admin");
            string errorMsg = string.Empty;
            // first step: check if username is admin (i.e. privileges of account)
            if (username.Equals("admin"))
            {
                // Retrieve the user profile using BProfileController
                var response = await _bProfileController.RetrieveProfileByUsername(username); // Call to BProfileController to fetch user profile

                if (response is OkObjectResult okResult && okResult.Value is Profile profile)
                {
                    Console.WriteLine($"Profile found for username: {profile.Username}");

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
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        errorMsg = "Password does not match with stored user password.";
                        Console.WriteLine(errorMsg);
                    }
                }
                else
                {
                    errorMsg = "Admin profile not found in database";
                    Console.WriteLine(errorMsg);
                }
            }
            else
            {
                errorMsg = "Admin profile not selected";
                Console.WriteLine(errorMsg);
            }

            // If invalid:
            ModelState.AddModelError("", $"Invalid login: {errorMsg}");
            Console.WriteLine("Login attempt invalid");
            return View("AdminLogin");
        }


        // called when user login is authenticated
        [HttpPost]
        public async Task<IActionResult> UserAuth(string username, string password)
        {
            string errorMsg = string.Empty;
            Console.WriteLine($"[LoginController] Attempting login for username: {username} with password: {password}");
            // Retrieve the user profile using BProfileController
            var response = await _bProfileController.RetrieveProfileByUsername(username); // Call to BProfileController to fetch user profile

            if (response is OkObjectResult okResult && okResult.Value is Profile profile)
            {
                Console.WriteLine($"Profile found for username: {profile.Username}");
                // Check if not admin
                if (!profile.Username.Equals("admin")) 
                {
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
                        errorMsg = "Password does not match with stored user password.";
                        Console.WriteLine(errorMsg);
                    }
                }
                else 
                {
                    errorMsg = "Admin account cannot login to user page.";
                    Console.WriteLine(errorMsg);
                }
            }
            else
            {
                errorMsg = "Profile not found for the provided username.";
                Console.WriteLine(errorMsg);
            }

            // If invalid:
            ModelState.AddModelError("", $"Invalid login: {errorMsg}");
            Console.WriteLine("Login attempt invalid");
            return View("UserLogin");
        }

        [HttpPost]
        public IActionResult UserLogout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return RedirectToAction("UserLogin", "Login");
        }
    }
}
