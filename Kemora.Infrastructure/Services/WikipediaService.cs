using Kemora.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kemora.Infrastructure.Services
{
    public class WikipediaService : IWikipediaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WikipediaService> _logger;

        public WikipediaService(IHttpClientFactory httpClientFactory, ILogger<WikipediaService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<string?> GetImageUrlAsync(string title)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Kemora/1.0 (mohamed@example.com)"); // Wikipedia requires a User-Agent

                var url = $"https://en.wikipedia.org/w/api.php?action=query&prop=pageimages&format=json&piprop=original&titles={Uri.EscapeDataString(title)}";
                
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                var pages = doc.RootElement.GetProperty("query").GetProperty("pages");
                foreach (var page in pages.EnumerateObject())
                {
                    if (page.Value.TryGetProperty("original", out var original))
                    {
                        return original.GetProperty("source").GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching image from Wikipedia for: {Title}", title);
            }

            return null;
        }
    }
}
