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
                var listLanguage = new List<object>
                {
                    new { Code = "vn", Name = "Tiếng Việt" },
                    new { Code = "en", Name = "English" },
                    new { Code = "fr", Name = "Français" },
                    new { Code = "ja", Name = "日本語" },
                    new { Code = "ko", Name = "한국어" },
                    new { Code = "zh", Name = "简体中文" },
                };
                var list = (await _serviceWrapper.DataDictionary.SelectAllAsync()).OrderByDescending(x => x.id);
               
                return Json(new
                {
                    data = list,
                    listLanguage = listLanguage
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
