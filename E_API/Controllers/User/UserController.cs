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
        [HttpGet("Select-User")]
        public async Task<IActionResult> GetUserCode([FromQuery] string userCode)
        {
            try
            {
                if (string.IsNullOrEmpty(userCode))
                    return BadRequest("User code cannot be null or empty.");

                var user = await _serviceWrapper.DataUser.SelectByCodeAsync(userCode);

                return OK(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
