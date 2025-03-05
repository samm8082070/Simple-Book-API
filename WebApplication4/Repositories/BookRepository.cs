using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using WebApplication4.Data;
using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public class BookRepository : IBookRepository
    {



        private readonly ApplicationDbContext _context; // Replace the list with DbContext
        private readonly ILogger<BookRepository> _logger; // Add ILogger

        public BookRepository(ApplicationDbContext context, ILogger<BookRepository> logger) // Inject DbContext
        {
            _context = context;
            _logger = logger;
        }
        public Book GetBook(int id)
        {
            try
            {
                return _context.Books
                    .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                    .SingleOrDefault(b => b.Id == id); // Use Find for primary key lookup
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID: {Id}", id); // Log the exception
                throw;
            }
        }

        public IEnumerable<Book> GetBooks()
        {
            try
            {
                return _context.Books
                    .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                    .ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving book IEnumerable");
                throw;
            }
        }

        public IEnumerable<Book> GetBooks(string? searchstring = null, int? numberofpages = null, DateOnly? publishdate = null, string? author = null)
        {
            
            IQueryable<Book> query = _context.Books;

            query = query
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre);

            if (!string.IsNullOrEmpty(searchstring))
            {
                //query = query.Where(b => b.Title.Contains(searchstring));
                query = query.Where(b => EF.Functions.Like(b.Title, $"%{searchstring}%")); // Parameterized
            }

            if (numberofpages.HasValue)
            {
                query = query.Where(b => b.NumberOfPages == numberofpages.Value);
            }

            if (publishdate.HasValue)
            {
                query = query.Where(b => b.PublishDate == publishdate.Value);
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));
            }

            var books = query.ToList();

            return books;
        }

        public Book AddBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                _context.SaveChanges(); // Save changes to the database
                return book;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error adding book ");
                throw;
            }
        }

        public Book UpdateBook(Book book)
        {
            try
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return book;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error updating book ");
                throw;
            }
        }

        public void DeleteBook(int id)
        {
            var bookToRemove = _context.Books.Find(id);
            if (bookToRemove != null)
            {
                try
                {
                    _context.Books.Remove(bookToRemove);
                    _context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "Error deleting book ");
                    throw;
                }
            }
        }
    }
}
