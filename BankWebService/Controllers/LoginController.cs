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
        [HttpPost]
        public IActionResult AdminAuth(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                // Set a session or cookie as needed
                Response.Cookies.Append("SessionID", "1234567");
                return RedirectToAction("Dashboard", "Admin");
            }

            // If login fails, return the login view with an error message
            ModelState.AddModelError("", "Invalid username or password.");
            return View("AdminLogin");
        }
    }
}
