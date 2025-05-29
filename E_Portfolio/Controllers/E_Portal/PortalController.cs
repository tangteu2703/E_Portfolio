using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.Portal
{
    public class PortalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        #region Portal
        public IActionResult Video()
        {
            return View();
        }
        public IActionResult Calendar()
        {
            return View();
        }
        public IActionResult Salary()
        {
            return View();
        }
        public IActionResult Chats()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult Organize()
        {
            return View();
        }
        public IActionResult Documents()
        {
            return View();
        }
        public IActionResult Tests()
        {
            return View();
        }
        #endregion
    }
}
