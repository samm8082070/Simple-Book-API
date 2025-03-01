using Humanizer.Localisation;

namespace WebApplication4.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public string Author { get; set; }

        public int NumberOfPages { get; set; }

        public DateOnly PublishDate { get; set; }
        
        public ICollection<BookGenre> BookGenres { get; set; } // Navigation property
    }
}
