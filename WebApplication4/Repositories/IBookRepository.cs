using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IBookRepository
    {
        Book GetBook(int id);
        IEnumerable<Book> GetBooks();
        void AddBook(Book book); // Example for adding
        void UpdateBook(Book book); // Example for updating
        void DeleteBook(int id);
    }
}
