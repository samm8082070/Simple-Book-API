using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IBookGenreRepository
    {
        Task AddGenreToBookAsync(int bookId, int genreId);
        Task RemoveGenreFromBookAsync(int bookId, int genreId);

        // Get all genres for a book
        Task<IEnumerable<Genre>> GetGenresForBookAsync(int bookId);

        // Check if a specific genre is associated with a book
        Task<bool> IsGenreAssignedToBookAsync(int bookId, int genreId);

        // Update genres for a book (replace existing genres with a new set)
        Task UpdateGenresForBookAsync(int bookId, IEnumerable<int> genreIds);

        // Get the BookGenre entity (if you need it directly)
        Task<BookGenre> GetBookGenreAsync(int bookId, int genreId); // Less common, but possible
    }
}
