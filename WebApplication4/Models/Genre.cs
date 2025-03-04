namespace WebApplication4.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }

        
        public ICollection<BookGenre> BookGenres { get; set; } // Navigation property
    }
}
