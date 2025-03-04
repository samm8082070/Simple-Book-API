using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IBookGenreRepository
    {
        void AddGenreToBook(int bookId, int genreId);
        void RemoveGenreFromBook(int bookId, int genreId);

        // Get all genres for a book
        IEnumerable<Genre> GetGenresForBook(int bookId);

        // Check if a specific genre is associated with a book
        bool IsGenreAssignedToBook(int bookId, int genreId);

        // Update genres for a book (replace existing genres with a new set)
        void UpdateGenresForBook(int bookId, IEnumerable<int> genreIds);

        // Get the BookGenre entity (if you need it directly)
        BookGenre GetBookGenre(int bookId, int genreId); // Less common, but possible
    }
}
