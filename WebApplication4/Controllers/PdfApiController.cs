using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.CodeAnalysis;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PdfApiController : ControllerBase
    {
        private readonly string _geminiApiEndpoint = "https://generativelanguage.googleapis.com";
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public PdfApiController(IMemoryCache cache , IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
        }
        [HttpGet("GetPdf/{id}")]
        public IActionResult GetPdf(int id)
        {
            string pdfFilePath = GetPdfFilePath(id);

            if (string.IsNullOrEmpty(pdfFilePath) || !System.IO.File.Exists(pdfFilePath))
            {
                return NotFound();
            }

            byte[] fileBytes;
            if (!_cache.TryGetValue(pdfFilePath, out fileBytes))
            {
                fileBytes = System.IO.File.ReadAllBytes(pdfFilePath);
                _cache.Set(pdfFilePath, fileBytes);
            }

            return File(fileBytes, "application/pdf");
        }

        private string GetPdfFilePath(int id)
        {
            // Your logic to get the pdf file path.
            // Example: return Path.Combine("path", id + ".pdf");
            return Path.Combine("wwwroot/pdfs/", id + ".pdf");
        }

        [HttpGet("GetSummary/{Id}")] // Route with ID parameter
        public async Task<IActionResult> GetSummary(int id)
        {

            var apiKey = _configuration["GeminiApiKey"];  // Assuming you have your API key in configuration
            var uploadUrl = "https://generativelanguage.googleapis.com/upload/v1beta/files?key=" + apiKey;
            var generateContentUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            try
            {
                // Construct the path to the PDF file
                string pdfFilePath = GetPdfFilePath(id);

                if (string.IsNullOrEmpty(pdfFilePath) || !System.IO.File.Exists(pdfFilePath))
                {
                    return NotFound();
                }

                var _httpClient =_httpClientFactory.CreateClient();
                string Uri = await UploadPdfFileAsync(_httpClient, uploadUrl, pdfFilePath);
                if (string.IsNullOrEmpty(Uri))
                {
                    return StatusCode(500, "Failed to upload PDF.");
                }
                // 2. Generate content (summarize)
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = "Give me a summary of this PDF file." },
                                new { fileData = new { mimeType = "application/pdf", fileUri = Uri } }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(generateContentUrl, content);
                response.EnsureSuccessStatusCode(); // Throw if not successful

                var responseContent = await response.Content.ReadAsStringAsync();
                // Deserialize the response and extract the text
                try
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var text = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();
                    return Ok(text);
                }
                catch (JsonException)
                {
                    // Handle the case where the response structure is not as expected
                    return StatusCode(500,"Failed to parse the summary from the response.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private static async Task<string> UploadPdfFileAsync(HttpClient httpClient, string uploadUrl, string pdfFilePath)
        {
            try
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfFilePath);
                var byteContent = new ByteArrayContent(fileBytes);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                var uploadResponse = await httpClient.PostAsync(uploadUrl, byteContent);
                uploadResponse.EnsureSuccessStatusCode();

                var uploadResponseContent = await uploadResponse.Content.ReadAsStringAsync();
                var uploadResult = JsonSerializer.Deserialize<JsonDocument>(uploadResponseContent);
                return uploadResult?.RootElement.GetProperty("file").GetProperty("uri").GetString();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"File Upload Failed (HttpRequestException): {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Failed: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during upload: {ex.Message}");
                return null;
            }
        }
    }
}
