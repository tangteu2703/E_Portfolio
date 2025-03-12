using E_Contract.Service;
using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.Dictionary
{
    [Route("[controller]")]
    public class DictionaryController : Controller
    {
        private readonly IServiceWrapper _serviceWrapper;
        public DictionaryController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SelectDictionary()
        {
            try
            {
                var list = (await _serviceWrapper.DataDictionary.SelectAllAsync()).OrderByDescending(x => x.id);
                return Json(new
                {
                    data = list,
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
