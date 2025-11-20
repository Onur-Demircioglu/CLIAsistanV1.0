using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CliAsistan.Models;

namespace CliAsistan.Services
{
    public class AIService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public AIService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public string GenerateResponse(List<Message> messages)
        {
            return GenerateResponseAsync(messages).GetAwaiter().GetResult();
        }

        public async Task<string> GenerateResponseAsync(List<Message> messages)
        {
            try
            {
                var requestBody = new
                {
                    model = _appSettings.ModelName,
                    messages = messages,
                    temperature = 0.7,
                    max_tokens = -1,
                    stream = false
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_appSettings.ApiUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    return $"Hata: Sunucu {response.StatusCode} döndürdü.";
                }

                var responseString = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message");
                    if (message.TryGetProperty("content", out var contentProp))
                    {
                        return contentProp.GetString() ?? "";
                    }
                }

                return "Hata: Anlamsız yanıt.";
            }
            catch (Exception ex)
            {
                return $"Bağlantı Hatası: {ex.Message}\n(LM Studio server'ın açık olduğundan emin ol: {_appSettings.ApiUrl})";
            }
        }
    }
}
