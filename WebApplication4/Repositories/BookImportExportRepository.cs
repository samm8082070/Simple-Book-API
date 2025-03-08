using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApplication4.Data;
using WebApplication4.Models;
using CsvHelper; 
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using static WebApplication4.Repositories.BookImportExportRepository;

namespace WebApplication4.Repositories
{
    public class BookImportExportRepository : IBookImportExportRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookImportExportRepository> _logger;
        private readonly IBookGenreRepository _bookGenreRepository;
        public BookImportExportRepository(ApplicationDbContext context, ILogger<BookImportExportRepository> logger, IBookGenreRepository bookGenreRepository)
        {
            _bookGenreRepository = bookGenreRepository;
            _context = context;
            _logger = logger;
        }

        public async Task ImportBooksFromCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<BookMap>();

            var bookRecords = new List<Book>(); // Create the list to hold the books

            // Read the CSV records one by one
            while (csv.Read())
            {
                var book = csv.GetRecord<Book>(); // Read the current book record

                if (book != null) // Check for null in case of errors
                {
                    bookRecords.Add(book);

                    var genreNames = GetGenreNamesFromCsvRow(csv); // Get the genres for the current book

                    foreach (var genreName in genreNames)
                    {
                        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.GenreName == genreName.Trim());

                        if (genre != null)
                        {
                            book.BookGenres = book.BookGenres ?? new List<BookGenre>(); // Initialize if needed
                            book.BookGenres.Add(new BookGenre { Book = book, Genre = genre });
                        }
                        else
                        {
                            _logger.LogWarning($"Genre '{genreName}' not found.");
                        }
                    }
                }
            }


            _context.Books.AddRange(bookRecords);
            await _context.SaveChangesAsync();
        }

        public class BookMap : ClassMap<Book>
        {
            public BookMap()
            {
                Map(m => m.Title).Name("Title");
                Map(m => m.Author).Name("Author");
                Map(m => m.NumberOfPages).Name("NumberOfPages");
                Map(m => m.PublishDate).Name("PublishDate").TypeConverterOption.Format("yyyy-MM-dd");
                // No mapping for BookGenres here
            }
        }

        private List<string> GetGenreNamesFromCsvRow(CsvReader csv)
        {
            var genreColumnValue = csv.GetField<string>("Genres");

            if (!string.IsNullOrEmpty(genreColumnValue))
            {
                return genreColumnValue.Split(',').Select(s => s.Trim()).ToList();
            }

            return new List<string>();
        }


        public async Task<string> ExportBooksToCsvAsync() // Changed return type to Task<string>
        {
            var books = await _context.Books
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .ToListAsync();

            using var stringWriter = new StringWriter(); // Use StringWriter instead of MemoryStream
            using var csv = new CsvWriter(stringWriter, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<BookExportMap>();
            csv.WriteHeader<BookCsvExport>();
            csv.NextRecord();

            var bookCsvExports = books.Select(book => new BookCsvExport
            {
                Title = book.Title,
                Author = book.Author,
                NumberOfPages = book.NumberOfPages,
                PublishDate = book.PublishDate,
                GenreNames = string.Join(",", book.BookGenres.Select(bg => bg.Genre.GenreName))
            }).ToList();

            csv.WriteRecords(bookCsvExports);

            return stringWriter.ToString(); // Return the CSV string
        }
        public class BookExportMap : ClassMap<BookCsvExport>
        {
            public BookExportMap()
            {
                Map(m => m.Title).Name("Title");
                Map(m => m.Author).Name("Author");
                Map(m => m.NumberOfPages).Name("NumberOfPages");
                Map(m => m.PublishDate).Name("PublishDate").TypeConverterOption.Format("yyyy-MM-dd");
                Map(m => m.GenreNames).Name("Genres");
            }
        }
        public class BookCsvExport
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public int NumberOfPages { get; set; }
            public DateOnly PublishDate { get; set; }
            public string GenreNames { get; set; }
        }
    }

}
