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
        public IActionResult Portfolio()
        {
            return View();
        }
        #endregion
    }
}
