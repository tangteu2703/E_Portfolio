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
        public IActionResult News()
        {
            return View();
        }
        public IActionResult TestSetup()
        {
            return View();
        }
        public IActionResult TestsHistory()
        {
            return View();
        }
        public IActionResult SalarySetup()
        {
            return View();
        }
        public IActionResult SalaryHistory()
        {
            return View();
        }
        public IActionResult Human()
        {
            return View();
        }
    }
}
