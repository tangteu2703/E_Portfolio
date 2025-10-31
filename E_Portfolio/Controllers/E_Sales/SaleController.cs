using Microsoft.AspNetCore.Mvc;
using E_Model.Sale;

namespace E_Portfolio.Controllers.E_Sales
{
    public class SaleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }
    }
}
