using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class PdfViewerController : Controller
    {
        [HttpGet("ViewPdf/{id}")]
        public IActionResult ViewPdf(int id)
        {
            
            ViewBag.BookId = id;
            return View();
        }
    }
}

