using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Model.Response
{
    [Serializable]
    public class ResponseBase<T>
    {
        public HttpStatusCode status_code { get; set; }
        public bool is_success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public int? total { get; set; }  // ✅ Thêm total để hỗ trợ phân trang

        public ResponseBase()
        { }

        public ResponseBase(HttpStatusCode status_code, T data, string message = "")
        {
            this.status_code = status_code;
            this.is_success = status_code == HttpStatusCode.OK;
            this.message = message;
            this.data = data;
        }

        public ResponseBase(HttpStatusCode status_code, string message = "")
        {
            this.status_code = status_code;
            this.is_success = status_code == HttpStatusCode.OK;
            this.message = message;
            if (data is IEnumerable<object> list)
            {
                total = list.Count(); // ✅ Nếu data là danh sách, lấy số lượng phần tử
            }
        }

        public ResponseBase(bool is_success, T data, string message = "")
        {
            this.status_code = is_success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            this.is_success = is_success;
            this.message = message;
            this.data = data;
        }

        public ResponseBase(bool is_success, string message = "")
        {
            this.status_code = is_success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            this.is_success = is_success;
            this.message = message;
        }

        public ContentResult ToContentResult()
        {
            return new ContentResult()
            {
                StatusCode = (int)this.status_code,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(this),
                ContentType = "json"
            };
        }

        public Task<ContentResult> ToContentResultAsync()
        {
            return Task.FromResult(new ContentResult()
            {
                StatusCode = (int)this.status_code,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(this),
                ContentType = "json"
            });
        }
    }

    public class ResponseBaseSuccess : ResponseBase<object>
    {
        public ResponseBaseSuccess(object data, string message = "")
        {
            this.status_code = HttpStatusCode.OK;
            this.is_success = true;
            this.message = message;
            this.data = data;
        }

        public ResponseBaseSuccess(string message = "")
        {
            this.status_code = HttpStatusCode.OK;
            this.is_success = true;
            this.message = message;
        }
    }

    public class ResponseBaseErr
    {
        public HttpStatusCode status_code { get; set; }
        public bool is_success { get; set; }
        public string message { get; set; }

        public ResponseBaseErr(string message = "")
        {
            this.status_code = HttpStatusCode.BadRequest;
            this.is_success = false;
            this.message = message;
        }

        public ContentResult ToContentResult()
        {
            return new ContentResult()
            {
                StatusCode = (int)this.status_code,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(this),
                ContentType = "json"
            };
        }

        public Task<ContentResult> ToContentResultAsync()
        {
            return Task.FromResult(new ContentResult()
            {
                StatusCode = (int)this.status_code,
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(this),
                ContentType = "json"
            });
        }
    }
}
