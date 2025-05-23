﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System;
using System.Net;
using System.Text;
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
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IBookImportExportRepository _importExportRepo;
        

        public BooksController(IBookRepository bookRepository, IGenreRepository genreRepository, IBookGenreRepository bookGenreRepository , IBookImportExportRepository importExportRepo)
        {
            _bookRepository = bookRepository;
            _genreRepository = genreRepository;
            _bookGenreRepository = bookGenreRepository;
            _importExportRepo = importExportRepo;
        }

        // Helper method to get the full error message
        private string GetFullErrorMessage(Exception ex)
        {
            string errorMessage = ex.Message;
            Exception innerException = ex.InnerException;

            while (innerException != null)
            {
                errorMessage += " --> " + innerException.Message;
                innerException = innerException.InnerException;
            }

            return errorMessage;
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

        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetBooks(string? searchstring = null, int? numberofpages = null, DateOnly? publishdate = null, string? author = null)
        {
            var books = _bookRepository.GetBooks(searchstring, numberofpages, publishdate, author);
            var mappedbookdtos = MappingsHelper(books);
            return Ok(mappedbookdtos); // Return data as JSON
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile importFile) {
            if (importFile == null)
            {
                return BadRequest("No file uploaded."); // Handle missing file
            }
            try
            {
                using var stream = importFile.OpenReadStream();
                await _importExportRepo.ImportBooksFromCsv(stream); // Use the new repo
                return Ok(new { message = "File imported successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            
        }

        [HttpPost("uploadimage/{bookId}")]
        public async Task<IActionResult> UploadImage(IFormFile coverImage , int bookId) {

            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // 1. Generate a unique filename
                var fileName = bookId + Path.GetExtension(coverImage.FileName);

                // 2. Construct the directory path
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/covers");

                // 3. Create the directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // 4. Construct the full file path
                var filePath = Path.Combine(directoryPath, fileName);

                // 5. Save the file to the file system
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(stream);
                }

                await _bookRepository.AddBookCoverAsync(bookId, "/images/covers/" + fileName);

                return Ok(new { message = "Book cover uploaded successfully!" });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportBooks()
        {
            var csvData = await _importExportRepo.ExportBooksToCsvAsync();

            return File(Encoding.UTF8.GetBytes(csvData), "text/csv", "books.csv"); // Return CSV file
        }
    }
}
