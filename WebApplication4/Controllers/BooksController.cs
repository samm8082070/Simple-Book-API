using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication4.Dtos;
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
        private readonly IBookRepository _bookRepository;
        private readonly IGenreRepository _genreRepository; 
        private readonly IBookGenreRepository _bookGenreRepository ; 

        public BooksController(IBookRepository bookRepository, IGenreRepository genreRepository, IBookGenreRepository bookGenreRepository)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _bookGenreRepository = bookGenreRepository;
        }
        private BookDto MappingHelper(Book book)
        {
            var bookDto = new BookDto  // Use BookDetailsDto (or appropriate DTO)
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                NumberOfPages = book.NumberOfPages,
                PublishDate = book.PublishDate,
                Genres = book.BookGenres.Select(bg => new GenreDto
                {
                    GenreId = bg.Genre.GenreId,
                    GenreName = bg.Genre.GenreName
                }).ToList()
            };
            return (bookDto); 
        }
        private static List<BookDto> MappingsHelper(IEnumerable<Book> books)
        {
            return books.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                NumberOfPages = book.NumberOfPages,
                PublishDate = book.PublishDate,
                Genres = book.BookGenres.Select(bg => new GenreDto
                {
                    GenreId = bg.Genre.GenreId,
                    GenreName = bg.Genre.GenreName
                }).ToList()
            }).ToList();
        }
        private BookEditDto MappingHelperEdit(Book book)
        {
            var bookEditDto = new BookEditDto  // Create a new BookEditDto
            {
                Id = book.Id,  // Map the Id
                Title = book.Title,  // Map the Title
                Author = book.Author, // Map the Author
                NumberOfPages = book.NumberOfPages, // Map the NumberOfPages
                PublishDate = book.PublishDate, // Map the PublishDate
                Genres = book.BookGenres.Select(bg => new GenreDto // Map the Genres 
                {
                    GenreId = bg.Genre.GenreId,
                    GenreName = bg.Genre.GenreName
                }).ToList(),
                AvailableGenres = _genreRepository.GetAllGenres() // Use the repository
                            .Select(g => new GenreDto
                            {
                                GenreId = g.GenreId,
                                GenreName = g.GenreName
                            })
                            .ToList()
            };
            return bookEditDto;
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
            else
            {
                List<BookDto> bookDtos = MappingsHelper(books);

                return Ok(bookDtos); // Return the DTOs
            }

        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = _bookRepository.GetBook(id); // Fetch the book with related data

            if (book == null)
            {
                return NotFound(); // Handle the case where the book doesn't exist
            }

            BookEditDto bookEditDto = MappingHelperEdit(book);

            return Ok(bookEditDto); // Return the DTO
        }
        
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            BookCreateDto bookCreateDto = new BookCreateDto
            {

                AvailableGenres = _genreRepository.GetAllGenres() // Use the repository
                            .Select(g => new GenreDto
                            {
                                GenreId = g.GenreId,
                                GenreName = g.GenreName
                            })
                            .ToList()
            };
            

            return Ok(bookCreateDto); // Return the DTO
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
            else
            {
                var bookdto = MappingHelper(book);
                return Ok(bookdto);
            } // Returns 200 OK with the book
        }


        // POST api/<BooksController>
        [HttpPost]
        public IActionResult Post([FromBody] BookCreateDto BookCreateDto)
        {
            var book = new Book
            {
                Id = BookCreateDto.Id,
                Title = BookCreateDto.Title,
                Author = BookCreateDto.Author,
                NumberOfPages = BookCreateDto.NumberOfPages,
                PublishDate = BookCreateDto.PublishDate

            };
            var bookReturned = _bookRepository.AddBook(book);

            var selectedGenreIds = BookCreateDto.SelectedGenreIds
            .Where(id => id != 00)
            .ToList();
            foreach (var item in selectedGenreIds)
            {
                _bookGenreRepository.AddGenreToBook(bookId: bookReturned.Id, genreId: item);
            }
            return CreatedAtAction(nameof(Get), new { id = bookReturned.Id }); // Return 201 Created with just the ID
        }

        // PUT api/<BooksController>/5
        [HttpPut]
        public IActionResult Put([FromBody] BookEditDto BookEditDto)
        {
            var book = new Book
            {
                Id = BookEditDto.Id,
                Title = BookEditDto.Title,
                Author = BookEditDto.Author,
                NumberOfPages = BookEditDto.NumberOfPages,
                PublishDate = BookEditDto.PublishDate

            };
            var bookReturned = _bookRepository.UpdateBook(book);
            var selectedGenreIds = BookEditDto.SelectedGenreIds
            .Where(id => id != 00)
            .ToList();
            foreach (var item in selectedGenreIds)
            {
                if (!_bookGenreRepository.IsGenreAssignedToBook(bookReturned.Id, item))
                { 
                    _bookGenreRepository.AddGenreToBook(bookId: bookReturned.Id, genreId: item);
                }
                
            }
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
