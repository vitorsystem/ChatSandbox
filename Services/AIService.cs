using System.Net.Http.Headers;
using System.Text.Json;
using DotNetEnv;

namespace ChatSandBox.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private string GetLocalizedErrorMessage(string userPhone, string errorType)
        {
            bool isBrazilian = userPhone.StartsWith("55");

            if (errorType == "Instability")
            {
                return isBrazilian
                    ? "Desculpe, estou passando por uma instabilidade no momento. Você poderia tentar novamente em alguns minutos?"
                    : "Sorry, I'm currently experiencing instability. Could you please try again in a few minutes?";
            }

            if (errorType == "EmptyOrBlocked")
            {
                return isBrazilian
                    ? "Desculpe, a mensagem está vazia ou foi bloqueada. Por favor, tente novamente."
                    : "Sorry, the message is empty or blocked. Please try again.";
            }

            return isBrazilian
                ? "Desculpe, ocorreu um erro interno. Tente novamente mais tarde."
                : "Sorry, an internal error occurred. Please try again later.";
        }

        public async Task<string> GetResponseAsync(string userPhoneNumber, string userMessage)
        {
            try
            {
                var url = "http://localhost:5678/webhook/chat";

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var requestBody = new
                {
                    message = userMessage,
                    phone = userPhoneNumber
                };

                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"ERRO DO N8N: {responseString}");
                    return "Erro interno, por favor tente novamente mais tarde!";    
                }

                using JsonDocument doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.TryGetProperty("aiResponse", out JsonElement content))
                {
                    return content.GetString() ?? "";
                }

                return "Mensagem não processada ou bloqueada. Tente novamente.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling n8n: {ex.Message}");
                return "Internal Error";
            }
        }
    }
}