using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PdfApiController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        public PdfApiController(IMemoryCache cache)
        {
            _cache = cache;
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
    }
}
