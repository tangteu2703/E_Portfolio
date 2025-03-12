using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.Authentication
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
