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
    }
}
