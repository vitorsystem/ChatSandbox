using ChatSandBox.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using DotNetEnv;

namespace ChatSandBox.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageController : ControllerBase
    {
        private readonly AiService _aiService;

        public MessageController(AiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("whatsapp")]
        [Consumes("application/json")]
        public async Task<IActionResult> ReceiveMessage([FromBody] JsonDocument payloadDoc)
        {
            try
            {
                var payload = payloadDoc.RootElement;

                if (payload.TryGetProperty("event", out var eventName) && eventName.GetString() == "messages.upsert")
                {
                    if (!payload.TryGetProperty("data", out var data)) return Ok();
                    if (!data.TryGetProperty("key", out var key)) return Ok();

                    if (key.TryGetProperty("fromMe", out var fromMe) && fromMe.GetBoolean()) return Ok();

                    string userPhone = "";
                    if (key.TryGetProperty("remoteJid", out var remoteJid))
                    {
                        userPhone = remoteJid.GetString()?.Split('@')[0] ?? "";
                    }

                    if (data.TryGetProperty("message", out var messageNode))
                    {
                        string userMessage = "";

                        if (messageNode.TryGetProperty("conversation", out var conv))
                        {
                            userMessage = conv.GetString();
                        }
                        else if (messageNode.TryGetProperty("extendedTextMessage", out var ext) && ext.TryGetProperty("text", out var textNode))
                        {
                            userMessage = textNode.GetString();
                        }

                        if (!string.IsNullOrEmpty(userMessage) && !string.IsNullOrEmpty(userPhone))
                        {
                            await ProcessAiAndReplyAsync(userPhone, userMessage);
                        }

                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when processing the message: {ex.Message}");
            }

            return Ok();
        }

        private async Task ProcessAiAndReplyAsync(string userPhone, string message)
        {
            try
            {
                string aiResponseText = await _aiService.GetResponseAsync(userPhone, message);

                using var httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("apikey", Env.GetString("AUTHENTICATION_API_KEY"));

                var requestBody = new
                {
                    number = userPhone,
                    text = aiResponseText
                };

                string evolutionUrl = "http://localhost:8080/message/sendText/ChatSandBox";

                var response = await httpClient.PostAsJsonAsync(evolutionUrl, requestBody);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Mensage send to Whatsapp with success!");
                else
                    Console.WriteLine($"Evolution API Error: {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error when sending message: {ex.Message}");
            }
        }

        [HttpGet("whatsapp")]
        public IActionResult HealthCheck()
        {
            return Ok("Nice Webhook!");
        }
    }
}
