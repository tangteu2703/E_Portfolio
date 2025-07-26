using E_Contract.Service;
using E_Model.Request.WorkSheet;
using E_Model.Table_SQL.WorkSheet;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Portfolio.Controllers.CMS
{
    public class CMSController : Controller
    {
        private readonly IServiceWrapper _serviceWrapper;
        public CMSController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Chatbot()
        {
            return View();
        }
        #region
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
        public IActionResult WorkSetup()
        {
            return View();
        }
        public IActionResult WorkSheetHistory()
        {
            return View();
        }
        public IActionResult WorkSheetDetail()
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
        #endregion
    }
}
