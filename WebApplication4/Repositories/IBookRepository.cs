using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IBookRepository
    {
        Book GetBook(int id);
        IEnumerable<Book> GetBooks();
        IEnumerable<Book> GetBooks(string? searchstring = null, int? numberofpages = null, DateOnly? publishdate = null, string? author = null);
        Book AddBook(Book book);
        Book UpdateBook(Book book); // Example for updating
        void DeleteBook(int id);
        public Task AddBookCoverAsync(int bookId, string coverImagePath);
    }
}
