namespace WebApplication4.Dtos
{
    public class BookEditDto
    {
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }
        public DateOnly PublishDate { get; set; }
        public List<int> SelectedGenreIds { get; set; } = new List<int>(); 
        public List<GenreDto> Genres { get; set; } = new List<GenreDto>(); // Genres associated with the book
        public List<GenreDto> AvailableGenres { get; set; } = new List<GenreDto>(); // All available genres
    }
}
