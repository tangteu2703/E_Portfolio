using E_Contract.Service;
using E_Model.Request;
using E_Model.Request.User;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public UserController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }
        [HttpGet("SelectAll")]
        public async Task<IActionResult> GetUserData([FromQuery] UserRequest request)
        {
            try
            {
                var listData = await _serviceWrapper.DataUser.SelectFilterAsync(request);

                return OK(listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
