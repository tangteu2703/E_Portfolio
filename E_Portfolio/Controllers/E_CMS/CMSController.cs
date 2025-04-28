using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.CMS
{
    public class CMSController : Controller
    {
        public IActionResult Index()
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
        public IActionResult Admin()
        {
            return View();
        }
    }
}
