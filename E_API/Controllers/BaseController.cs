using E_Contract.Service;
using E_Model.Response;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_API.Controllers
{
    public class BaseController : ControllerBase
    {
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
            var result = new ResponseBaseErr(message).ToContentResult();
            result.StatusCode = (int)HttpStatusCode.BadRequest;
            return result;
        }

        protected Task<ContentResult> BadRequestAsync(string message = "")
        {
            var result = new ResponseBaseErr(message).ToContentResultAsync();
            return SetStatusCodeAsync(result, HttpStatusCode.BadRequest);
        }

        protected ContentResult NotFound(string message = "Not found")
        {
            var result = new ResponseBaseErr(message).ToContentResult();
            result.StatusCode = (int)HttpStatusCode.NotFound;
            return result;
        }

        protected ContentResult Unauthorized(string message = "Unauthorized")
        {
            var result = new ResponseBaseErr(message).ToContentResult();
            result.StatusCode = (int)HttpStatusCode.Unauthorized;
            return result;
        }

        protected ContentResult InternalServerError(string message = "Internal server error")
        {
            var result = new ResponseBaseErr(message).ToContentResult();
            result.StatusCode = (int)HttpStatusCode.InternalServerError;
            return result;
        }
        protected ContentResult InternalServerError(string message = "Internal server error", Exception? ex = null)
        {
            var result = new ResponseBaseErr(message, (int)HttpStatusCode.InternalServerError).ToContentResult();
            result.StatusCode = (int)HttpStatusCode.InternalServerError;

            return result;
        }
        private async Task<ContentResult> SetStatusCodeAsync(Task<ContentResult> task, HttpStatusCode code)
        {
            var result = await task;
            result.StatusCode = (int)code;
            return result;
        }
    }
}