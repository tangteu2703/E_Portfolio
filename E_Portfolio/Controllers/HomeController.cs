using System.Diagnostics;
using E_Portfolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Portfolio()
        {
            return View();
        }
    }
}
