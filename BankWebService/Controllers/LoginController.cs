using Microsoft.AspNetCore.Mvc;

namespace BankWebService.Controllers
{
    public class LoginController : Controller
    {
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
        public IActionResult UserAuth(string id, string password)
        {
            // Profile profile = JsonConvert<Profile>(ProfileController.GetProfile(id)) - might not be the exact code... 
            
            if (id == "admin" && password == "admin") // basic, one account check.
            // if (profile != null && id == Profile.id && password == Profile.password)
            {
                var sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("SessionID", sessionId, new CookieOptions
                {
                    HttpOnly = true, // Helps mitigate XSS attacks
                    Secure = true, // Ensures the cookie is only sent over HTTPS
                    SameSite = SameSiteMode.Strict // Protects against CSRF attacks
                });
            }

            // If invalid:
            ModelState.AddModelError("", "Invalid account ID or password.");
            return View("UserLogin");
        }
    }
}
