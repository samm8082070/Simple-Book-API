namespace WebApplication4.Dtos
{
    public class BookCreateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }
        public DateOnly PublishDate { get; set; }
        public List<int> SelectedGenreIds { get; set; } = new List<int>(); 
        public List<GenreDto> AvailableGenres { get; set; } = new List<GenreDto>(); // All available genres
    }
}
