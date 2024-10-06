using Microsoft.AspNetCore.Mvc;

namespace BankBusinessService.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
