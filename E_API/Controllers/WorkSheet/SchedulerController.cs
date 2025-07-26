using E_Contract.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.WorkSheet
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public SchedulerController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        [HttpGet("Task-Historied")]
        public async Task<IActionResult> SelectTaskHistoried()
        {
            try
            {
                var jobList = (await _serviceWrapper.TaskHistoried.SelectAllAsync("E_PortalConnection")).ToList();

                return Ok(jobList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }
    }
}
