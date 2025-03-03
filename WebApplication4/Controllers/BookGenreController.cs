using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using WebApplication4.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookGenreApiController : ControllerBase
    {
        private readonly IBookGenreRepository _bookGenreRepository;

        public BookGenreApiController(IBookGenreRepository bookGenreRepository)
        {
            _bookGenreRepository = bookGenreRepository;
        }

        [HttpPost("books/{bookId}/genres/{genreId}")]
        public async Task<IActionResult> AddGenreToBook(int bookId, int genreId)
        {
            await _bookGenreRepository.AddGenreToBookAsync(bookId, genreId);
            return Ok(); // Or CreatedAtActionResult
        }

        [HttpDelete("books/{bookId}/genres/{genreId}")]
        public async Task<IActionResult> RemoveGenreFromBook(int bookId, int genreId)
        {
            await _bookGenreRepository.RemoveGenreFromBookAsync(bookId, genreId);
            return NoContent();
        }

        [HttpGet("books/{bookId}/genres")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenresForBook(int bookId)
        {
            var genres = await _bookGenreRepository.GetGenresForBookAsync(bookId);
            return Ok(genres);
        }

        [HttpGet("books/{bookId}/genres/{genreId}")]
        public async Task<IActionResult> IsGenreAssignedToBook(int bookId, int genreId)
        {
            var isAssigned = await _bookGenreRepository.IsGenreAssignedToBookAsync(bookId, genreId);
            return Ok(isAssigned);
        }

        [HttpPut("books/{bookId}/genres")]
        public async Task<IActionResult> UpdateGenresForBook(int bookId, [FromBody] List<int> genreIds)
        {
            await _bookGenreRepository.UpdateGenresForBookAsync(bookId, genreIds);
            return NoContent();
        }
    }
}

