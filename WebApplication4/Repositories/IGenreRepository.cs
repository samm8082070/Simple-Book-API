using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public interface IGenreRepository
    {
        IEnumerable<Genre> GetAllGenres();
        Genre GetGenreById(int id);
        void AddGenre(Genre genre);
        void UpdateGenre(Genre genre);
        void DeleteGenre(int id);
    }
}
