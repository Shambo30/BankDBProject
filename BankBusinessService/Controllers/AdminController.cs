using Microsoft.AspNetCore.Mvc;

namespace BankBusinessService.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
