using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.CMS
{
    public class CMSController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin()
        {
            return View();
        }
    }
}
