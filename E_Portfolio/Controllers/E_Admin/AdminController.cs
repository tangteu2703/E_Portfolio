using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.User
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
