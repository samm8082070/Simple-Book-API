using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Repositories
{
    public class BookGenreRepository : IBookGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public BookGenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public  void AddGenreToBook(int bookId, int genreId)
        {
            var bookGenre = new BookGenre { BookId = bookId, GenreId = genreId };
            _context.BookGenres.Add(bookGenre);
             _context.SaveChanges();
        }

        public void  RemoveGenreFromBook(int bookId, int genreId)
        {
            var bookGenre =  _context.BookGenres.Find(bookId, genreId);
            if (bookGenre != null)
            {
                _context.BookGenres.Remove(bookGenre);
                 _context.SaveChanges();
            }
        }
        public  IEnumerable<Genre> GetGenresForBook(int bookId)
        {
            return  _context.BookGenres
                .Where(bg => bg.BookId == bookId)
                .Include(bg => bg.Genre) // Important: Include the Genre entity
                .Select(bg => bg.Genre) // Select only the Genre entities
                .ToList();
        }

        public  bool IsGenreAssignedToBook(int bookId, int genreId)
        {
            return  _context.BookGenres.Any(bg => bg.BookId == bookId && bg.GenreId == genreId);
        }

        public  void UpdateGenresForBook(int bookId, IEnumerable<int> genreIds)
        {
            // 1. Remove existing genres for the book
            var existingGenres =  _context.BookGenres.Where(bg => bg.BookId == bookId).ToList();
            _context.BookGenres.RemoveRange(existingGenres);

            // 2. Add the new genres
            foreach (var genreId in genreIds)
            {
                var bookGenre = new BookGenre { BookId = bookId, GenreId = genreId };
                _context.BookGenres.Add(bookGenre);
            }

             _context.SaveChanges();
        }

        public  BookGenre GetBookGenre(int bookId, int genreId)
        {
            return  _context.BookGenres.Find(bookId, genreId);
        }
    }

}
