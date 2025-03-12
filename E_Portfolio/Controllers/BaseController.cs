using E_Contract.Service;
using E_Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IServiceWrapper _serviceWrapper;

        public BaseController(IServiceWrapper serviceWrapper)
        {
            this._serviceWrapper = serviceWrapper;
        }

        protected ContentResult OK()
        {
            return new ResponseBaseSuccess().ToContentResult();
        }

        protected ContentResult OK(object data)
        {
            return new ResponseBaseSuccess(data).ToContentResult();
        }

        protected Task<ContentResult> OKAsync(object data)
        {
            return new ResponseBaseSuccess(data).ToContentResultAsync();
        }

        protected ContentResult BadRequest(string message = "")
        {
            return new ResponseBaseErr(message).ToContentResult();
        }

        protected Task<ContentResult> BadRequestAsync(string message = "")
        {
            return new ResponseBaseErr(message).ToContentResultAsync();
        }
    }
}