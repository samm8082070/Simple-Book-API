using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Models;
using WebApplication4.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository; // Repository field
        
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/<BooksController>
        [HttpGet]
        public IActionResult Get()
        {
            var books = _bookRepository.GetBooks();
            if (books == null)
            {
                return NotFound();
            }
            else {
                return Ok(books); // Returns 200 OK with the list of books
            }
            
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = _bookRepository.GetBook(id);
            if (book == null)
            {
                return NotFound(); // Returns 404 Not Found if the book doesn't exist
            }
            return Ok(book); // Returns 200 OK with the book
        }

        // POST api/<BooksController>
        [HttpPost]
        public IActionResult Post([FromBody] Book book)
        {
            _bookRepository.AddBook(book);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book); // Returns 201 Created with the new book
        }

        // PUT api/<BooksController>/5
        [HttpPut]
        public IActionResult Put([FromBody] Book book)
        {
            _bookRepository.UpdateBook(book);
            return NoContent(); // Returns 204 No Content for successful updates
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _bookRepository.DeleteBook(id);
            return NoContent(); // Returns 204 No Content for successful deletions
        }
    }
}
