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

        public async Task AddGenreToBookAsync(int bookId, int genreId)
        {
            var bookGenre = new BookGenre { BookId = bookId, GenreId = genreId };
            _context.BookGenres.Add(bookGenre);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveGenreFromBookAsync(int bookId, int genreId)
        {
            var bookGenre = await _context.BookGenres.FindAsync(bookId, genreId);
            if (bookGenre != null)
            {
                _context.BookGenres.Remove(bookGenre);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Genre>> GetGenresForBookAsync(int bookId)
        {
            return await _context.BookGenres
                .Where(bg => bg.BookId == bookId)
                .Include(bg => bg.Genre) // Important: Include the Genre entity
                .Select(bg => bg.Genre) // Select only the Genre entities
                .ToListAsync();
        }

        public async Task<bool> IsGenreAssignedToBookAsync(int bookId, int genreId)
        {
            return await _context.BookGenres.AnyAsync(bg => bg.BookId == bookId && bg.GenreId == genreId);
        }

        public async Task UpdateGenresForBookAsync(int bookId, IEnumerable<int> genreIds)
        {
            // 1. Remove existing genres for the book
            var existingGenres = await _context.BookGenres.Where(bg => bg.BookId == bookId).ToListAsync();
            _context.BookGenres.RemoveRange(existingGenres);

            // 2. Add the new genres
            foreach (var genreId in genreIds)
            {
                var bookGenre = new BookGenre { BookId = bookId, GenreId = genreId };
                _context.BookGenres.Add(bookGenre);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<BookGenre> GetBookGenreAsync(int bookId, int genreId)
        {
            return await _context.BookGenres.FindAsync(bookId, genreId);
        }
    }

}
