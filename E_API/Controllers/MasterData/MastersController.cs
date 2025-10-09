using E_Contract.Service;
using E_Model.Request.WorkSheet;
using E_Model.Table_SQL.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                var version =  new
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

        [HttpGet("departments")]
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var listData = (await _serviceWrapper.DataUser.SelectAllAsync())
                                .Select(c => new { c.user_code, c.full_name })
                                .ToList();

                return OK(listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DeviceType")]
        public async Task<IActionResult> GetDeviceTypes()
        {
            try
            {
                var listData = (await _serviceWrapper.DeviceType.SelectAllAsync("E_PortalConnection"))
                                .Select(c => new { c.type_id, c.type_name })
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
