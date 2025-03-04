using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IBookRepository
    {
        Book GetBook(int id);
        IEnumerable<Book> GetBooks();
        Book AddBook(Book book);
        Book UpdateBook(Book book); // Example for updating
        void DeleteBook(int id);
    }
}
