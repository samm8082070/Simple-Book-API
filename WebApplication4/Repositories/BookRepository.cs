using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
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
                return _context.Books.Find(id); // Use Find for primary key lookup
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
                return _context.Books.ToList(); // Use ToList to get all books
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving book IEnumerable");
                throw;
            }
        }

        public void AddBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                _context.SaveChanges(); // Save changes to the database
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error adding book ");
                throw;
            }
        }

        public void UpdateBook(Book book)
        {
            try
            {
                _context.Books.Update(book);
                _context.SaveChanges();
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
        //private readonly List<Book> _books = new List<Book> // Your data store (replace with DB)
        //{
        //    new Book { Id = 1, Title = "The Lord of the Rings" },
        //    new Book { Id = 2, Title = "The Hitchhiker's Guide to the Galaxy" }
        //};


        //public Book GetBook(int id)
        //{
        //    return _books.FirstOrDefault(b => b.Id == id); // Use FirstOrDefault to handle not found
        //}

        //public IEnumerable<Book> GetBooks()
        //{
        //    return _books;
        //}

        //public void AddBook(Book book)
        //{
        //    _books.Add(book);
        //}

        //public void UpdateBook(Book book)
        //{
        //    var existingBook = _books.Find(b => b.Id == book.Id);
        //    if (existingBook != null)
        //    {
        //        // Update the properties of the existingBook with the values from the book parameter
        //        existingBook.Title = book.Title;
        //        // ... update other properties
        //    }
        //}

        //public void DeleteBook(int id)
        //{
        //    var bookToRemove = _books.Find(b => b.Id == id);
        //    if (bookToRemove != null)
        //    {
        //        _books.Remove(bookToRemove);
        //    }
        //}
    }
}
