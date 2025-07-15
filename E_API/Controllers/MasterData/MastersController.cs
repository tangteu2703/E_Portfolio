using E_Contract.Service;
using E_Model.Request.WorkSheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.MasterData
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public MastersController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }


        [HttpGet("Version-API")]
        public async Task<IActionResult> GetVersions()
        {
            try
            {
                var version = new
                {
                    Version = "20250714",
                    BuildDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
                };
                return OK(version);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Departments")]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var listData = (await _serviceWrapper.Department.SelectAllAsync())
                                .Select(c => new { c.dept_code, c.dept_name })
                                .ToList();

                return OK(listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
