using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace E_API.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatbotController()
        {
            _httpClient = new HttpClient();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest input)
        {
            var rasaUrl = "http://localhost:5005/webhooks/rest/webhook"; // API của Rasa

            var payload = new
            {
                sender = input.Sender ?? "default_user",
                message = input.Message
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(rasaUrl, content);
            var responseText = await response.Content.ReadAsStringAsync();

            return Ok(responseText);
        }
    }
    public class ChatRequest
    {
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}
