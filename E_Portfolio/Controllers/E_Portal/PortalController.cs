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
        public IActionResult Portal()
        {
            return View();
        }
        public IActionResult Video()
        {
            return View();
        }
        public IActionResult Calendar()
        {
            return View();
        }
        public IActionResult Tasks()
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
        #endregion
    }
}
