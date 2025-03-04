using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public    IEnumerable<Genre> GetAllGenres  ()
        {
            return  _context.Genres.ToList  ();
        }

        public    Genre GetGenreById  (int id)
        {
            return  _context.Genres.Find  (id);
        }

        public    void AddGenre  (Genre genre)
        {
            _context.Genres.Add(genre);
             _context.SaveChanges  ();
        }

        public    void UpdateGenre  (Genre genre)
        {
            _context.Entry(genre).State = EntityState.Modified; // Important for updates
             _context.SaveChanges  ();
        }

        public    void DeleteGenre  (int id)
        {
            var genre =  _context.Genres.Find  (id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                 _context.SaveChanges  ();
            }
        }
    }
}
