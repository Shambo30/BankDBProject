using Microsoft.AspNetCore.Mvc;

namespace BankWebService.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
