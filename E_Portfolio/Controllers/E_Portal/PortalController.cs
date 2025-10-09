using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.Portal
{
    public class PortalController : Controller
    {
        [Route("Portal")]
        public IActionResult Index()
        {
            return View();
        }
        #region Portal
        [Route("Page-Video")]
        public IActionResult Video()
        {
            return View();
        }
        [Route("Page-WorkPlans")]
        public IActionResult Calendar()
        {
            return View();
        }
        [Route("Page-Salary")]
        public IActionResult Salary()
        {
            return View();
        }
        [Route("Page-Chats")]
        public IActionResult Chats()
        {
            return View();
        }
        [Route("Page-Account")]
        public IActionResult Users()
        {
            return View();
        }
        [Route("Page-Menu")]
        public IActionResult Menu()
        {
            return View();
        }
        [Route("Page-Organize")]
        public IActionResult Organize()
        {
            return View();
        }
        [Route("Page-Documents")]
        public IActionResult Documents()
        {
            return View();
        }
        [Route("Page-Tests")]
        public IActionResult Tests()
        {
            return View();
        }
        #endregion
    }
}
