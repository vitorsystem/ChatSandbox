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
                var apiKey = Env.GetString("GROQ_API_KEY");

                var url = "https://api.groq.com/openai/v1/chat/completions";

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "openai/gpt-oss-120b",
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    }
                };

                request.Content = JsonContent.Create(requestBody);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GROQ ERROR: {responseString}");
                    return GetLocalizedErrorMessage(userPhoneNumber, "Instability");
                }

                using JsonDocument doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.TryGetProperty("choices", out JsonElement choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("message", out JsonElement message) &&
                        message.TryGetProperty("content", out JsonElement content))
                    {
                        string aiResponseText = content.GetString() ?? "";

                        if (string.IsNullOrWhiteSpace(aiResponseText))
                        {
                            return GetLocalizedErrorMessage(userPhoneNumber, "EmptyOrBlocked");
                        }

                        Console.WriteLine(aiResponseText);
                        return aiResponseText;
                    }
                }

                return GetLocalizedErrorMessage(userPhoneNumber, "InternalError");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when calling AI API: {ex.Message}");
                return GetLocalizedErrorMessage(userPhoneNumber, "InternalError");
            }
        }
    }
}