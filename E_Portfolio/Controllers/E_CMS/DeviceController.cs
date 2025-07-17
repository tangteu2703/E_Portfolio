using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.E_CMS
{
    public class DeviceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DeviceRequest()
        {
            return View();
        }
        public IActionResult DeviceManagement()
        {
            return View();
        }
        public IActionResult DeviceHistory()
        {
            return View();
        }
    }
}
