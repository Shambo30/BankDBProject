using Microsoft.AspNetCore.Mvc;

namespace BankWebService.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
